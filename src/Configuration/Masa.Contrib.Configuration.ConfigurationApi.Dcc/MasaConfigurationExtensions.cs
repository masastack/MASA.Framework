// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
        var configurationExpandSection = builder.Configuration.GetSection("ExpandSections");
        if (configurationExpandSection.Exists())
        {
            configurationExpandSection.Bind(expandSections);
        }

        return builder.UseDcc(() => dccOptions, option =>
        {
            option.Environment = builder.Configuration[nameof(DccSectionOptions.Environment)];
            option.Cluster = builder.Configuration[nameof(DccSectionOptions.Cluster)];
            option.AppId = StaticConfig.AppId;
            option.ConfigObjects = builder.Configuration.GetSection(nameof(DccSectionOptions.ConfigObjects)).Get<List<string>>();
            option.Secret = builder.Configuration[nameof(DccSectionOptions.Secret)];
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
        Action<CallerOptions>? callerOptions)
    {
        StaticConfig.AppId = defaultSectionOptions.AppId;

        var services = builder.Services;
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();

        var config = GetDccConfigurationOption(configureOptions, defaultSectionOptions, expansionSectionOptions);

        var jsonSerializerOption = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions?.Invoke(jsonSerializerOption);
        services.AddCaller(options =>
        {
            if (callerOptions == null)
            {
                options.UseHttpClient(()
                    => new MasaHttpClientBuilder(DEFAULT_CLIENT_NAME, string.Empty,
                        opt => opt.BaseAddress = new Uri(config.DccConfigurationOptions.ManageServiceAddress))
                );
            }
            else
            {
                callerOptions.Invoke(options);
            }
        });

        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, config.DccConfigurationOptions.RedisOptions)
            .AddSharedMasaMemoryCache(config.DccConfigurationOptions.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX);

        TryAddConfigurationApiClient(services, config.DefaultSectionOptions, config.ExpansionSectionOptions, jsonSerializerOption);
        TryAddConfigurationApiManage(services, config.DefaultSectionOptions, config.ExpansionSectionOptions);

        var sectionOptions = new List<DccSectionOptions>()
        {
            config.DefaultSectionOptions
        }.Concat(config.ExpansionSectionOptions);

        var configurationApiClient = services.BuildServiceProvider().GetRequiredService<IConfigurationApiClient>();
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(sectionOptions, configurationApiClient, loggerFactory));
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
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
            return DccFactory.CreateManage(callerFactory, defaultSectionOption, expansionSectionOptions);
        });
        return services;
    }

    private static DccOptions GetDccConfigurationOption(
        DccConfigurationOptions dccConfigurationOption,
        DccSectionOptions defaultSectionOptions,
        List<DccSectionOptions>? expansionSectionOptions = null)
    {
        ArgumentNullException.ThrowIfNull(dccConfigurationOption, nameof(dccConfigurationOption));

        if (string.IsNullOrEmpty(dccConfigurationOption.ManageServiceAddress))
            throw new ArgumentNullException(nameof(dccConfigurationOption.ManageServiceAddress));

        if (dccConfigurationOption.RedisOptions == null)
            throw new ArgumentNullException(nameof(dccConfigurationOption.RedisOptions));

        if (dccConfigurationOption.RedisOptions.Servers == null || dccConfigurationOption.RedisOptions.Servers.Count == 0 ||
            dccConfigurationOption.RedisOptions.Servers.Any(service => string.IsNullOrEmpty(service.Host) || service.Port <= 0))
            throw new ArgumentNullException(nameof(dccConfigurationOption.RedisOptions.Servers));

        ArgumentNullException.ThrowIfNull(defaultSectionOptions, nameof(defaultSectionOptions));

        if (string.IsNullOrEmpty(defaultSectionOptions.AppId))
            throw new ArgumentNullException("AppId cannot be empty");

        if (defaultSectionOptions.ConfigObjects == null || !defaultSectionOptions.ConfigObjects.Any())
            throw new ArgumentNullException("ConfigObjects cannot be empty");

        if (string.IsNullOrEmpty(defaultSectionOptions.Cluster))
            defaultSectionOptions.Cluster = "Default";
        if (string.IsNullOrEmpty(defaultSectionOptions.Environment))
            defaultSectionOptions.Environment = GetDefaultEnvironment();

        List<DccSectionOptions> expansionOptions = new();
        foreach (var expansionOption in expansionSectionOptions ?? new())
        {
            if (string.IsNullOrEmpty(expansionOption.Environment))
                expansionOption.Environment = defaultSectionOptions.Environment;
            if (string.IsNullOrEmpty(expansionOption.Cluster))
                expansionOption.Cluster = defaultSectionOptions.Cluster;

            if (expansionOption.ConfigObjects == null || !expansionOption.ConfigObjects.Any())
                throw new ArgumentNullException("ConfigObjects in the extension section cannot be empty");

            if (expansionOption.AppId == defaultSectionOptions.AppId ||
                expansionOptions.Any(section => section.AppId == expansionOption.AppId))
                throw new ArgumentNullException("The current section already exists, no need to mount repeatedly");

            expansionOptions.Add(expansionOption);
        }
        return new(dccConfigurationOption, defaultSectionOptions, expansionOptions);
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

    private static string GetDefaultEnvironment()
        => System.Environment.GetEnvironmentVariable(DEFAULT_ENVIRONMENT_NAME) ??
            throw new ArgumentNullException(
                "Error getting environment information, please make sure the value of ASPNETCORE_ENVIRONMENT has been configured");

    private class DccConfigurationProvider
    {

    }
}
