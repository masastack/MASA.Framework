namespace MASA.Contrib.BasicAbility.Dcc;

public static class MasaConfigurationExtensions
{
    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        IServiceCollection services,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null)
        => builder.UseDcc(services, "Appsettings", jsonSerializerOptions, callerOptions);

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        IServiceCollection services,
        string defaultSectionName,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null)
    {
        if (!builder.GetSectionRelations().TryGetValue(defaultSectionName, out IConfiguration? configuration))
            throw new ArgumentNullException("Failed to obtain Dcc configuration, check whether the current section is configured with Dcc");

        var configurationSection = configuration.GetSection("DccOptions");
        var dccOptions = configurationSection.Get<DccConfigurationOptions>();

        List<DccSectionOptions> expandSections = new();
        var configurationExpandSection = configuration.GetSection("ExpandSections");
        if (configurationExpandSection.Exists())
        {
            configurationExpandSection.Bind(expandSections);
        }

        return builder.UseDcc(services, () => dccOptions, option =>
        {
            option.Environment = configuration["Environment"];
            option.Cluster = configuration["Cluster"];
            option.AppId = configuration["AppId"];
            option.ConfigObjects = configuration.GetSection("ConfigObjects").Get<List<string>>();
            option.Secret = configuration["Sectet"];
        }, option => option.ExpandSections = expandSections, jsonSerializerOptions, callerOptions);
    }

    public static IMasaConfigurationBuilder UseDcc(
        this IMasaConfigurationBuilder builder,
        IServiceCollection services,
        Func<DccConfigurationOptions> configureOptions,
        Action<DccSectionOptions> defaultSectionOptions,
        Action<DccExpandSectionOptions>? expansionSectionOptions = null,
        Action<JsonSerializerOptions>? jsonSerializerOptions = null,
        Action<CallerOptions>? callerOptions = null)
    {
        if (services.Any(service => service.ImplementationType == typeof(DccConfigurationProvider)))
            return builder;

        services.AddSingleton<DccConfigurationProvider>();

        var config = GetDccConfigurationOption(configureOptions, defaultSectionOptions, expansionSectionOptions);

        var jsonSerializerOption = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };
        jsonSerializerOptions?.Invoke(jsonSerializerOption);
        services.AddCaller(Options =>
        {
            if (callerOptions == null)
            {
                Options.UseHttpClient(()
                    => new MasaHttpClientBuilder(DEFAULT_CLIENT_NAME, string.Empty, opt => opt.BaseAddress = new Uri(config.DccConfigurationOption.ManageServiceAddress), jsonSerializerOption)
                );
            }
            else
            {
                callerOptions.Invoke(Options);
            }
        });

        services.AddMasaRedisCache(DEFAULT_CLIENT_NAME, config.DccConfigurationOption.RedisOptions).AddSharedMasaMemoryCache(config.DccConfigurationOption.SubscribeKeyPrefix ?? DEFAULT_SUBSCRIBE_KEY_PREFIX);

        TryAddConfigurationAPIClient(services, config.DefaultSectionOption, config.ExpansionSectionOptions, jsonSerializerOption);
        TryAddConfigurationAPIManage(services, config.DefaultSectionOption, config.ExpansionSectionOptions);

        var sectionOptions = new List<DccSectionOptions>()
        {
            config.DefaultSectionOption
        }.Concat(config.ExpansionSectionOptions);

        var configurationAPIClient = services.BuildServiceProvider().GetRequiredService<IConfigurationAPIClient>();
        var loggerFactory = services.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        builder.AddRepository(new DccConfigurationRepository(sectionOptions, configurationAPIClient, loggerFactory));
        return builder;
    }

    public static IServiceCollection TryAddConfigurationAPIClient(IServiceCollection services,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions> expansionSectionOptions,
        JsonSerializerOptions jsonSerializerOption)
    {
        services.TryAddSingleton(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IMemoryCacheClientFactory>()
                .CreateClient(DEFAULT_CLIENT_NAME);

            if (client == null)
                throw new ArgumentNullException(nameof(client));

            return DccFactory.CreateClient(client, jsonSerializerOption, defaultSectionOption, expansionSectionOptions);
        });
        return services;
    }

    public static IServiceCollection TryAddConfigurationAPIManage(IServiceCollection services,
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

    private static (DccSectionOptions DefaultSectionOption, List<DccSectionOptions> ExpansionSectionOptions, DccConfigurationOptions DccConfigurationOption) GetDccConfigurationOption(
        Func<DccConfigurationOptions> configureOptions,
        Action<DccSectionOptions> defaultSectionOptions,
        Action<DccExpandSectionOptions>? expansionSectionOptions = null)
    {
        var dccConfigurationOption = configureOptions?.Invoke() ?? null;
        if (dccConfigurationOption == null)
            throw new ArgumentNullException(nameof(configureOptions));

        if (string.IsNullOrEmpty(dccConfigurationOption.ManageServiceAddress))
            throw new ArgumentNullException(nameof(dccConfigurationOption.ManageServiceAddress));

        if (dccConfigurationOption.RedisOptions == null)
            throw new ArgumentNullException(nameof(dccConfigurationOption.RedisOptions));

        if (dccConfigurationOption.RedisOptions.Servers == null || dccConfigurationOption.RedisOptions.Servers.Count == 0 || dccConfigurationOption.RedisOptions.Servers.Any(service => string.IsNullOrEmpty(service.Host) || service.Port <= 0))
            throw new ArgumentNullException(nameof(dccConfigurationOption.RedisOptions.Servers));

        if (defaultSectionOptions == null)
            throw new ArgumentNullException(nameof(defaultSectionOptions));

        var defaultSectionOption = new DccSectionOptions();
        defaultSectionOptions.Invoke(defaultSectionOption);

        if (string.IsNullOrEmpty(defaultSectionOption.AppId))
            throw new ArgumentNullException("AppId cannot be empty");

        if (defaultSectionOption.ConfigObjects == null || !defaultSectionOption.ConfigObjects.Any())
            throw new ArgumentNullException("ConfigObjects cannot be empty");

        if (string.IsNullOrEmpty(defaultSectionOption.Cluster))
            defaultSectionOption.Cluster = "Default";
        if (string.IsNullOrEmpty(defaultSectionOption.Environment))
            defaultSectionOption.Environment = GetDefaultEnvironment();

        var dccCachingOption = new DccExpandSectionOptions();
        expansionSectionOptions?.Invoke(dccCachingOption);
        List<DccSectionOptions> expansionOptions = new();
        foreach (var expansionOption in dccCachingOption.ExpandSections ?? new())
        {
            if (string.IsNullOrEmpty(expansionOption.Environment))
                expansionOption.Environment = defaultSectionOption.Environment;
            if (string.IsNullOrEmpty(expansionOption.Cluster))
                expansionOption.Cluster = defaultSectionOption.Cluster;

            if (expansionOption.ConfigObjects == null || !expansionOption.ConfigObjects.Any())
                throw new ArgumentNullException("ConfigObjects in the extension section cannot be empty");

            if (expansionOption.AppId == defaultSectionOption.AppId || expansionOptions.Any(section => section.AppId == expansionOption.AppId))
                throw new ArgumentNullException("The current section already exists, no need to mount repeatedly");

            expansionOptions.Add(expansionOption);
        }
        return (defaultSectionOption, expansionOptions, dccConfigurationOption);
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
        throw new ArgumentNullException("Error getting environment information, please make sure the value of ASPNETCORE_ENVIRONMENT has been configured");

    private class DccConfigurationProvider
    {

    }
}
