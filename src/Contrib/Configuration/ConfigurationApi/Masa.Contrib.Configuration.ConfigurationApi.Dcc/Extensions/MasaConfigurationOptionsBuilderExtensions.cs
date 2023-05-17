// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.Configuration;

public static class MasaConfigurationOptionsBuilderExtensions
{
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

    public static void UseDcc(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        string sectionName = "DccOptions",
        Action<JsonSerializerOptions>? jsonSerializerOptionsConfigure = null,
        Action<CallerBuilder>? action = null)
    {
        if (!masaConfigurationOptionsBuilder.TryUseDccCore(jsonSerializerOptionsConfigure, action, null))
            return;

        // When using the configuration node, the Dcc configuration will be taken from the local configuration
        masaConfigurationOptionsBuilder.AddRegistrationOptions(new ConfigurationRelationOptions()
        {
            IsRequiredConfigComponent = true,
            SectionType = SectionTypes.Local,
            Section = sectionName,
            ObjectType = typeof(DccOptions)
        });
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
            .TryAddDccConfigurationOptionProvider();

        masaConfigurationOptionsBuilder.Services.Configure<MasaConfigurationOptions>(options =>
        {
            options.AddConfigurationRepository(
                SectionTypes.ConfigurationApi,
                serviceProvider =>
                {
                    var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                        .GetOptions(serviceProvider, dccOptionsFunc);

                    var configurationApiClient = ConfigurationApiFactory.CreateClient(
                        CacheUtils.CreateMultilevelCacheClient(dccConfigurationOptions, serviceProvider),
                        serviceProvider.GetService<ILogger<ConfigurationApiClient>>(),
                        jsonSerializerOption,
                        dccConfigurationOptions
                    );

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
                    var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                        .GetOptions(serviceProvider, dccOptionsFunc);

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

    internal static MasaConfigurationOptionsBuilder AddCaching(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.AddMultilevelCache(
            DEFAULT_CLIENT_NAME,
            distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(serviceProvider =>
            {
                var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                    .GetOptions(serviceProvider, dccOptionsFunc);

                return dccConfigurationOptions.RedisOptions;
            }),
            serviceProvider =>
            {
                var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                    .GetOptions(serviceProvider, dccOptionsFunc);

                var multilevelCacheGlobalOptions = new MultilevelCacheGlobalOptions
                {
                    SubscribeKeyType = SubscribeKeyType.SpecificPrefix,
                    SubscribeKeyPrefix = dccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX
                };

                return multilevelCacheGlobalOptions;
            });
        return masaConfigurationOptionsBuilder;
    }

    internal static MasaConfigurationOptionsBuilder TryAddConfigurationApiClient(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        JsonSerializerOptions jsonSerializerOption,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.TryAddSingleton<SingletonService<IConfigurationApiClient>>(serviceProvider
            => new SingletonService<IConfigurationApiClient>(GetConfigurationApiClient(serviceProvider)));

        masaConfigurationOptionsBuilder.Services.TryAddScoped<ScopedService<IConfigurationApiClient>>(serviceProvider
            => new ScopedService<IConfigurationApiClient>(GetConfigurationApiClient(serviceProvider)));
        return masaConfigurationOptionsBuilder;

        IConfigurationApiClient GetConfigurationApiClient(IServiceProvider serviceProvider)
        {
            var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                .GetOptions(serviceProvider, dccOptionsFunc);

            return ConfigurationApiFactory.CreateClient(
                CacheUtils.CreateMultilevelCacheClient(dccConfigurationOptions, serviceProvider),
                serviceProvider.GetService<ILogger<ConfigurationApiClient>>(),
                jsonSerializerOption,
                dccConfigurationOptions);
        }
    }

    internal static MasaConfigurationOptionsBuilder TryAddConfigurationApiManage(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder,
        JsonSerializerOptions jsonSerializerOptions,
        Func<IServiceProvider, DccOptions>? dccOptionsFunc)
    {
        masaConfigurationOptionsBuilder.Services.TryAddSingleton(serviceProvider =>
        {
            var dccConfigurationOptions = serviceProvider.GetRequiredService<DccConfigurationOptionProvider>()
                .GetOptions(serviceProvider, dccOptionsFunc);

            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return ConfigurationApiFactory.CreateManage(
                callerFactory.Create(DEFAULT_CLIENT_NAME),
                jsonSerializerOptions,
                dccConfigurationOptions);
        });
        return masaConfigurationOptionsBuilder;
    }

    internal static MasaConfigurationOptionsBuilder TryAddDccConfigurationOptionProvider(
        this MasaConfigurationOptionsBuilder masaConfigurationOptionsBuilder)
    {
        masaConfigurationOptionsBuilder.Services.TryAddTransient<DccConfigurationOptionsCache>();

        masaConfigurationOptionsBuilder.Services.TryAddSingleton<SingletonService<DccConfigurationOptionProvider>>(serviceProvider =>
        {
            var dccConfigurationOptionProvider =
                new DccConfigurationOptionProvider(serviceProvider.GetRequiredService<DccConfigurationOptionsCache>());
            return new SingletonService<DccConfigurationOptionProvider>(dccConfigurationOptionProvider);
        });
        masaConfigurationOptionsBuilder.Services.TryAddScoped<ScopedService<DccConfigurationOptionProvider>>(serviceProvider =>
        {
            var dccConfigurationOptionProvider =
                new DccConfigurationOptionProvider(serviceProvider.GetRequiredService<DccConfigurationOptionsCache>());
            return new ScopedService<DccConfigurationOptionProvider>(dccConfigurationOptionProvider);
        });

        masaConfigurationOptionsBuilder.Services.TryAddScoped<DccConfigurationOptionProvider>(serviceProvider =>
        {
            var enableMultiEnvironment = serviceProvider.GetRequiredService<IOptions<IsolationOptions>>().Value.EnableMultiEnvironment;
            return enableMultiEnvironment ?
                serviceProvider.GetRequiredService<ScopedService<DccConfigurationOptionProvider>>().Service :
                serviceProvider.GetRequiredService<SingletonService<DccConfigurationOptionProvider>>().Service;
        });
        return masaConfigurationOptionsBuilder;
    }

    private sealed class DccConfigurationProvider
    {

    }
}
