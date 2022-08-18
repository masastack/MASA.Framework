// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.Extensions.Options;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc;

public static class MasaConfigurationExtensions
{
    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null)
    {
        var configurationSection = builder.Configuration.GetSection("DccOptions");
        var dccOptions = configurationSection.Get<DccConfigurationOptions>();

        List<DccSectionOptions> expandSections = new();
        var configurationExpandSection = configurationSection.GetSection("ExpandSections");
        if (configurationExpandSection.Exists())
        {
            configurationExpandSection.Bind(expandSections);
        }

        return builder.UseDcc(() => dccOptions, option =>
        {
            option.Environment = configurationSection[nameof(DccSectionOptions.Environment)];
            option.Cluster = configurationSection[nameof(DccSectionOptions.Cluster)];
            option.AppId = configurationSection[nameof(DccSectionOptions.AppId)];
            option.ConfigObjects = configurationSection.GetSection(nameof(DccSectionOptions.ConfigObjects)).Get<List<string>>();
            option.Secret = configurationSection[nameof(DccSectionOptions.Secret)];
        }, option => option.ExpandSections = expandSections, jsonSerializerOptions, callerOptions);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        Func<DccConfigurationOptions> configureOptions,
        Action<DccSectionOptions> defaultSectionOptions,
        Action<DccExpandSectionOptions>? expansionSectionOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null)
    {
        ArgumentNullException.ThrowIfNull(configureOptions, nameof(configureOptions));
        DccConfigurationOptions dccConfigurationOptions = configureOptions.Invoke();

        ArgumentNullException.ThrowIfNull(defaultSectionOptions, nameof(defaultSectionOptions));
        DccSectionOptions defaultSectionOption = new();
        defaultSectionOptions.Invoke(defaultSectionOption);

        var expansionSectionOption = new DccExpandSectionOptions();
        expansionSectionOptions?.Invoke(expansionSectionOption);

        return builder.UseDcc(dccConfigurationOptions, defaultSectionOption, expansionSectionOption.ExpandSections,
            jsonSerializerOptions, callerOptions);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        DccConfigurationOptions configureOptions,
        DccSectionOptions defaultSectionOptions,
        List<DccSectionOptions>? expansionSectionOptions,
        Action<JsonSerializerOptions>? jsonSerializerOptions,
        Action<CallerOptions>? action)
    {
        var services = builder.Services;
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();

        var config = GetDccConfigurationOption(builder, configureOptions, defaultSectionOptions, expansionSectionOptions);

        StaticConfig.AppId = defaultSectionOptions.AppId;
        StaticConfig.PublicId = configureOptions.PublicId;

        var jsonSerializerOption = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions?.Invoke(jsonSerializerOption);
        string callerName = DEFAULT_CLIENT_NAME;
        services.AddCaller(options =>
        {
            options.Assemblies = new[] { typeof(DccConfigurationProvider).Assembly };
            if (action == null)
            {
                options.UseHttpClient(callerName, ()
                    => new MasaHttpClientBuilder(
                        opt => opt.BaseAddress = new Uri(config.DccConfigurationOptions.ManageServiceAddress))
                );
            }
            else
            {
                action.Invoke(options);
                callerName = options.Callers.Select(opt => opt.Name).FirstOrDefault()
                    ?? throw new Exception("Missing Caller implementation, eg: options.UseHttpClient()");
            }
        });

        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, config.DccConfigurationOptions.RedisOptions)
            .AddSharedMasaMemoryCache(config.DccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX);

        TryAddConfigurationApiClient(services, config.DefaultSectionOptions, config.ExpansionSectionOptions, jsonSerializerOption);
        TryAddConfigurationApiManage(services, callerName, config.DefaultSectionOptions, config.ExpansionSectionOptions);

        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);
        GetDccSectionOptionsByAppidPattern(client, defaultSectionOptions.AppId);

        var configurationApiClient = serviceProvider.GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var cacheClient = serviceProvider.GetRequiredService<IDistributedCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);
        builder.AddRepository(new DccConfigurationRepository(config.DefaultSectionOptions, config.ExpansionSectionOptions, configurationApiClient, loggerFactory, cacheClient));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationApiClient(IServiceCollection services,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>().CreateClient(DEFAULT_CLIENT_NAME);

            ArgumentNullException.ThrowIfNull(client, nameof(client));

            return DccFactory.CreateClient(
                serviceProvider,
                client,
                jsonSerializerOption,
                defaultSectionOption,
                expansionSectionOptions);
        });
        return services;
    }

    public static IServiceCollection TryAddConfigurationApiManage(IServiceCollection services,
        string callerName,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateManage(callerFactory.Create(callerName), defaultSectionOption, expansionSectionOptions);
        });
        return services;
    }

    private static List<string> GetDccSectionOptionsByAppidPattern(IDistributedCacheClient client, string appId)
    {
        List<string> keys = client.GetKeys($"*{appId}*");

        return keys;
    }

    private static DccOptions GetDccConfigurationOption(
        IMasaConfigurationBuilder builder,
        DccConfigurationOptions dccConfigurationOption,
        DccSectionOptions defaultSectionOptions,
        List<DccSectionOptions>? expansionSectionOptions = null)
    {
        ArgumentNullException.ThrowIfNull(dccConfigurationOption, nameof(dccConfigurationOption));

        if (string.IsNullOrEmpty(dccConfigurationOption.ManageServiceAddress))
            throw new ArgumentNullException(nameof(dccConfigurationOption.ManageServiceAddress));

        ArgumentNullException.ThrowIfNull(dccConfigurationOption.RedisOptions, nameof(dccConfigurationOption.RedisOptions));

        if (dccConfigurationOption.RedisOptions.Servers == null || !dccConfigurationOption.RedisOptions.Servers.Any() ||
            dccConfigurationOption.RedisOptions.Servers.Any(service => string.IsNullOrEmpty(service.Host) || service.Port <= 0))
            throw new ArgumentNullException(nameof(dccConfigurationOption.RedisOptions.Servers));

        var masaAppConfigureOptions =
            builder.Services.BuildServiceProvider().GetRequiredService<IOptions<MasaAppConfigureOptions>>();

        dccConfigurationOption.PublicId ??= masaAppConfigureOptions.Value.Data.GetValueOrDefault(nameof(DccConfigurationOptions.PublicId))
                ?? StaticConfig.PublicId;
        dccConfigurationOption.PublicSecret ??= masaAppConfigureOptions.Value.Data.GetValueOrDefault(nameof(DccConfigurationOptions.PublicSecret));

        ArgumentNullException.ThrowIfNull(defaultSectionOptions, nameof(defaultSectionOptions));

        if (string.IsNullOrEmpty(defaultSectionOptions.AppId))
            defaultSectionOptions.AppId = masaAppConfigureOptions.Value.AppId;
        if (string.IsNullOrEmpty(defaultSectionOptions.Cluster))
            defaultSectionOptions.Cluster = masaAppConfigureOptions.Value.Cluster;
        if (string.IsNullOrEmpty(defaultSectionOptions.Environment))
            defaultSectionOptions.Environment = masaAppConfigureOptions.Value.Environment;
        if (string.IsNullOrEmpty(defaultSectionOptions.Secret))
            defaultSectionOptions.Secret = masaAppConfigureOptions.Value.Data.GetValueOrDefault(nameof(DccSectionOptions.Secret));
        defaultSectionOptions.ConfigObjects ??= new();

        List<DccSectionOptions> expandSections = new()
        {
            //add public SectionOptions to expandSections
            new DccSectionOptions
            {
                Environment = defaultSectionOptions.Environment,
                Cluster = defaultSectionOptions.Cluster,
                AppId = dccConfigurationOption.PublicId,
                Secret = dccConfigurationOption.PublicSecret
                ?? masaAppConfigureOptions.Value.Data.GetValueOrDefault(nameof(DccConfigurationOptions.PublicSecret)),
                ConfigObjects = new()
            }
        };
        foreach (var expansionOption in expansionSectionOptions ?? new())
        {
            expansionOption.ConfigObjects ??= new();
            if (string.IsNullOrEmpty(expansionOption.Environment))
                expansionOption.Environment = defaultSectionOptions.Environment;
            if (string.IsNullOrEmpty(expansionOption.Cluster))
                expansionOption.Cluster = defaultSectionOptions.Cluster;

            if (expansionOption.AppId == defaultSectionOptions.AppId ||
                expandSections.Any(section => section.AppId == expansionOption.AppId))
                throw new ArgumentNullException("The current section already exists, no need to mount repeatedly");

            expandSections.Add(expansionOption);
        }
        return new(dccConfigurationOption, defaultSectionOptions, expandSections);
    }

    private static ICachingBuilder AddSharedMasaMemoryCache(this ICachingBuilder builder, string subscribeKeyPrefix)
    {
        builder.AddMasaMemoryCache(options =>
        {
            options.SubscribeKeyType = SubscribeKeyTypes.SpecificPrefix;
            options.SubscribeKeyPrefix = subscribeKeyPrefix;
        });

        return builder;
    }

    private sealed class DccConfigurationProvider
    {

    }
}
