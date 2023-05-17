// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class MasaConfigurationOptionsBuilderExtensions
{
    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        string sectionName = "DccOptions",
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        if (!masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, null))
            return;

        // When using the configuration node, the Dcc configuration will be taken from the local configuration

        masaConfigurationOptionsBuilder.Services.Configure<ConfigurationAutoMapOptions>(options =>
        {
            if (options.Data.All(item => item.ObjectType != typeof(DccOptions)))
            {
                options.Data.Add(new ConfigurationRelationOptions()
                {
                    IsRequiredConfigComponent = true,
                    SectionType = SectionTypes.Local,
                    Section = sectionName,
                    ObjectType = typeof(DccOptions),
                });
            }
        });
    }

    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        DccOptions dccOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, _ => dccOptions);
    }

    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Action<DccOptions> dccOptionsFun,
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, _ =>
        {
            var dccOptions = new DccOptions();
            dccOptionsFun.Invoke(dccOptions);
            return dccOptions;
        });
    }

    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Action<DccOptions, IServiceProvider> dccOptionsFun,
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, serviceProvider =>
        {
            var dccOptions = new DccOptions();
            dccOptionsFun.Invoke(dccOptions, serviceProvider);
            return dccOptions;
        });
    }

    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Func<IServiceProvider, DccOptions> dccOptionsFun,
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, dccOptionsFun);
    }

    private static bool TryUseDccCore(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure,
        Action<CallerBuilder>? action,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        if (masaConfigurationOptionsBuilder.Services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return false;

        masaConfigurationOptionsBuilder.Services.AddSingleton<DccConfigurationProvider>();

        var jsonSerializerOption = GetJsonSerializerOptions(jsonSerializerOptionsConfigure);

        masaConfigurationOptionsBuilder
            .AddCaller(action, dccOptionsFunc)
            .AddCaching(dccOptionsFunc)
            .TryAddConfigurationApiClient(jsonSerializerOption, dccOptionsFunc)
            .TryAddConfigurationApiManage(jsonSerializerOption, dccOptionsFunc)
            .AddDccConfigurationOptionProvider()
            .ComplementAppIdByAutoMapOptions(dccOptionsFunc);

        masaConfigurationOptionsBuilder.Services.AddTransient<IConfigurationApi, DefaultConfigurationApi>();

        // Add a remote config provider to the list of config providers
        masaConfigurationOptionsBuilder.Services.Configure<MasaConfigurationOptions>(options =>
        {
            options.AddConfigurationRepository(
                SectionTypes.ConfigurationApi,
                serviceProvider =>
                {
                    var dccConfigurationOptions =
                        serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

                    var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();

                    var configurationRepository = new DccConfigurationRepository(
                        dccConfigurationOptions.GetAllAvailabilitySections(),
                        configurationApiClient,
                        serviceProvider.GetService<ILoggerFactory>());
                    return configurationRepository;
                });
        });
        return true;
    }

    private static JsonSerializerOptions GetJsonSerializerOptions(Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null)
    {
        var globalJsonSerializerOptions = MasaApp.GetJsonSerializerOptions();
        var jsonSerializerOption = globalJsonSerializerOptions != null ?
            new JsonSerializerOptions(globalJsonSerializerOptions)
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            } :
            new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

        jsonSerializerOptionsConfigure?.Invoke(jsonSerializerOption);
        return jsonSerializerOption;
    }

    #region Dcc configuration dependent program

    #region Caller

    internal static MasaConfigurationOptionsBuilder AddCaller(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Action<CallerBuilder>? action,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        var callerName = DEFAULT_CLIENT_NAME;
        masaConfigurationOptionsBuilder.Services.AddCaller(callerName, options =>
        {
            if (action == null)
            {
                options.UseHttpClient((client, serviceProvider) =>
                {
                    var dccConfigurationOptions =
                        serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

                    client.BaseAddress = dccConfigurationOptions.ManageServiceAddress;
                });
            }
            else
            {
                action.Invoke(options);
            }
        });
        return masaConfigurationOptionsBuilder;
    }

    #endregion

    #region Caching

    internal static MasaConfigurationOptionsBuilder AddCaching(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.AddMultilevelCache(
            DEFAULT_CLIENT_NAME,
            distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(serviceProvider =>
            {
                var dccConfigurationOptions =
                    serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

                return dccConfigurationOptions.RedisOptions;
            }),
            serviceProvider =>
            {
                var dccConfigurationOptions =
                    serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

                var multilevelCacheGlobalOptions = new MultilevelCacheGlobalOptions
                {
                    SubscribeKeyType = SubscribeKeyType.SpecificPrefix,
                    SubscribeKeyPrefix = dccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX
                };

                return multilevelCacheGlobalOptions;
            });
        return masaConfigurationOptionsBuilder;
    }

    #endregion

    #region ConfigurationApiClient

    internal static MasaConfigurationOptionsBuilder TryAddConfigurationApiClient(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        JsonSerializerOptions jsonSerializerOption,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.TryAddSingleton<SingletonService<IConfigurationApiClient>>(serviceProvider
            => new SingletonService<IConfigurationApiClient>(GetConfigurationApiClient(serviceProvider)));

        masaConfigurationOptionsBuilder.Services.TryAddScoped<ScopedService<IConfigurationApiClient>>(serviceProvider
            => new ScopedService<IConfigurationApiClient>(GetConfigurationApiClient(serviceProvider)));

        masaConfigurationOptionsBuilder.Services.TryAddTransient<IConfigurationApiClient>(serviceProvider =>
            serviceProvider.EnableMultiEnvironment() ?
                serviceProvider.GetRequiredService<ScopedService<IConfigurationApiClient>>().Service :
                serviceProvider.GetRequiredService<SingletonService<IConfigurationApiClient>>().Service);

        return masaConfigurationOptionsBuilder;

        IConfigurationApiClient GetConfigurationApiClient(IServiceProvider serviceProvider)
        {
            var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

            var manualMultilevelCacheClient =
                CacheUtils.CreateMultilevelCacheClient(dccConfigurationOptions, serviceProvider.GetService<IFormatCacheKeyProvider>());

            return new ConfigurationApiClient(
                manualMultilevelCacheClient,
                jsonSerializerOption,
                dccConfigurationOptions,
                serviceProvider.GetService<ILogger<ConfigurationApiClient>>());
        }
    }

    #endregion

    #region ConfigurationApiManage

    internal static MasaConfigurationOptionsBuilder TryAddConfigurationApiManage(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        JsonSerializerOptions jsonSerializerOptions,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.TryAddSingleton<SingletonService<IConfigurationApiManage>>(serviceProvider
            => new SingletonService<IConfigurationApiManage>(GetConfigurationApiManage(serviceProvider)));

        masaConfigurationOptionsBuilder.Services.TryAddScoped<ScopedService<IConfigurationApiManage>>(serviceProvider
            => new ScopedService<IConfigurationApiManage>(GetConfigurationApiManage(serviceProvider)));

        masaConfigurationOptionsBuilder.Services.TryAddTransient<IConfigurationApiManage>(serviceProvider =>
            serviceProvider.EnableMultiEnvironment() ?
                serviceProvider.GetRequiredService<ScopedService<IConfigurationApiManage>>().Service :
                serviceProvider.GetRequiredService<SingletonService<IConfigurationApiManage>>().Service);

        return masaConfigurationOptionsBuilder;

        IConfigurationApiManage GetConfigurationApiManage(IServiceProvider serviceProvider)
        {
            var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>().GetOptions(dccOptionsFunc);

            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();

            return new ConfigurationApiManage(
                callerFactory.Create(DEFAULT_CLIENT_NAME),
                jsonSerializerOptions,
                dccConfigurationOptions);
        }
    }

    #endregion

    #region Dcc configuration information

    /// <summary>
    /// Add a Dcc configuration provider for obtaining Dcc configuration information
    /// </summary>
    /// <param name="masaConfigurationOptionsBuilder"></param>
    /// <returns></returns>
    internal static MasaConfigurationOptionsBuilder AddDccConfigurationOptionProvider(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        masaConfigurationOptionsBuilder.Services.AddSingleton<DccConfigurationOptionsCache>();

        masaConfigurationOptionsBuilder.Services.AddSingleton<SingletonService<DccConfigurationOptionProvider>>(serviceProvider =>
        {
            var dccConfigurationOptionProvider = new DccConfigurationOptionProvider(serviceProvider);
            return new SingletonService<DccConfigurationOptionProvider>(dccConfigurationOptionProvider);
        });
        masaConfigurationOptionsBuilder.Services.AddScoped<ScopedService<DccConfigurationOptionProvider>>(serviceProvider =>
        {
            var dccConfigurationOptionProvider = new DccConfigurationOptionProvider(serviceProvider);
            return new ScopedService<DccConfigurationOptionProvider>(dccConfigurationOptionProvider);
        });

        masaConfigurationOptionsBuilder.Services.AddTransient<DccConfigurationOptionProvider>(serviceProvider =>
        {
            var enableMultiEnvironment = serviceProvider.EnableMultiEnvironment();
            return enableMultiEnvironment ?
                serviceProvider.GetRequiredService<ScopedService<DccConfigurationOptionProvider>>().Service :
                serviceProvider.GetRequiredService<SingletonService<DccConfigurationOptionProvider>>().Service;
        });
        return masaConfigurationOptionsBuilder;
    }

    #endregion

    #region Perfect AppId

    /// <summary>
    /// Supplement and improve the automatic mapping of empty AppId
    /// </summary>
    internal static void ComplementAppIdByAutoMapOptions(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.AddSingleton<SingletonService<IAutoMapOptionsByConfigurationApiProvider>>(serviceProvider
            =>
        {
            var autoMapOptionsProvider = new DefaultAutoMapOptionsByConfigurationApiProvider(serviceProvider, dccOptionsFunc);
            return new SingletonService<IAutoMapOptionsByConfigurationApiProvider>(autoMapOptionsProvider);
        });
        masaConfigurationOptionsBuilder.Services.AddScoped<ScopedService<IAutoMapOptionsByConfigurationApiProvider>>(serviceProvider =>
        {
            var autoMapOptionsProvider = new DefaultAutoMapOptionsByConfigurationApiProvider(serviceProvider, dccOptionsFunc);
            return new ScopedService<IAutoMapOptionsByConfigurationApiProvider>(autoMapOptionsProvider);
        });
        masaConfigurationOptionsBuilder.Services.AddTransient<IAutoMapOptionsByConfigurationApiProvider>(serviceProvider =>
            serviceProvider.EnableMultiEnvironment() ?
                serviceProvider.GetRequiredService<ScopedService<IAutoMapOptionsByConfigurationApiProvider>>().Service :
                serviceProvider.GetRequiredService<SingletonService<IAutoMapOptionsByConfigurationApiProvider>>().Service);
    }

    #endregion

    #endregion

    private sealed class DccConfigurationProvider
    {

    }
}
