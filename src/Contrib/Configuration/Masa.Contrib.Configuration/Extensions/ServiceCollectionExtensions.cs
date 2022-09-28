// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection InitializeAppConfiguration(this IServiceCollection services)
        => services.InitializeAppConfiguration(null);

    public static IServiceCollection InitializeAppConfiguration(
        this IServiceCollection services,
        Action<MasaAppConfigureOptionsRelation>? action)
    {
        if (services.Any(service => service.ImplementationType == typeof(InitializeAppConfigurationProvider)))
            return services;

        services.AddSingleton<InitializeAppConfigurationProvider>();

        MasaApp.TrySetServiceCollection(services);

        MasaAppConfigureOptionsRelation optionsRelation = new();
        action?.Invoke(optionsRelation);
        IConfiguration? migrateConfiguration = null;
        bool initialized = false;

        services.Configure<MasaAppConfigureOptions>(options =>
        {
            if (!initialized)
            {
                var masaConfiguration = services.BuildServiceProvider().GetService<IMasaConfiguration>();
                if (masaConfiguration != null) migrateConfiguration = masaConfiguration.Local;
                initialized = true;
            }
            var sourceConfiguration = services.BuildServiceProvider().GetService<IConfiguration>();

            if (string.IsNullOrWhiteSpace(options.AppId))
                options.AppId = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.AppId)),
                    sourceConfiguration,
                    migrateConfiguration);

            if (string.IsNullOrWhiteSpace(options.Environment))
                options.Environment = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.Environment)),
                    sourceConfiguration,
                    migrateConfiguration);

            if (string.IsNullOrWhiteSpace(options.Cluster))
                options.Cluster = GetConfigurationValue(
                    optionsRelation.GetValue(nameof(options.Cluster)),
                    sourceConfiguration,
                    migrateConfiguration);

            foreach (var key in optionsRelation.GetKeys())
            {
                options.TryAdd(key, GetConfigurationValue(
                    optionsRelation.GetValue(key),
                    sourceConfiguration,
                    migrateConfiguration));
            }
        });
        return services;
    }

    private static string GetConfigurationValue((string Variable, string DefaultValue) defaultConfig,
        IConfiguration? configuration,
        IConfiguration? migrateConfiguration)
    {
        var value = string.Empty;
        if (configuration != null)
        {
            value = configuration[defaultConfig.Variable];
            if (!string.IsNullOrWhiteSpace(value))
                return value;
        }

        if (migrateConfiguration != null)
            value = migrateConfiguration[defaultConfig.Variable];
        if (string.IsNullOrWhiteSpace(value))
            value = defaultConfig.DefaultValue;
        return value;
    }

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate = null)
    {
        Action<ConfigurationOptions>? action = null;
        return services.AddMasaConfiguration(configureDelegate, action);
    }

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        params Assembly[] assemblies)
        => services.AddMasaConfiguration(
            null,
            options => options.Assemblies = assemblies);

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        Action<ConfigurationOptions>? action)
        => services.AddMasaConfiguration(null, action);

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        params Assembly[] assemblies)
        => services.AddMasaConfiguration(
            configureDelegate,
            options => options.Assemblies = assemblies);

    public static IServiceCollection AddMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        Action<ConfigurationOptions>? action)
    {
        services.InitializeAppConfiguration();

        var sourceConfiguration = services.BuildServiceProvider().GetService<IConfiguration>();

        var configurationBuilder = sourceConfiguration as IConfigurationBuilder ??
            (sourceConfiguration == null ? new ConfigurationBuilder() : new ConfigurationBuilder().AddConfiguration(sourceConfiguration));

        IConfigurationRoot masaConfiguration =
            services.CreateMasaConfiguration(
                configureDelegate,
                configurationBuilder,
                action);

        if (!masaConfiguration.Providers.Any())
            return services;

        configurationBuilder.Sources.Clear();
        configurationBuilder.AddConfiguration(masaConfiguration);

        if (sourceConfiguration == null)
            services.AddSingleton<IConfiguration>(_ => configurationBuilder.Build());

        return services;
    }

    public static IMasaConfiguration GetMasaConfiguration(this IServiceCollection services)
        => services.BuildServiceProvider().GetRequiredService<IMasaConfiguration>();

    public static IConfigurationRoot CreateMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        IConfigurationBuilder configurationBuilder,
        params Assembly[] assemblies)
    {
        return services.CreateMasaConfiguration(
            configureDelegate,
            configurationBuilder,
            options => options.Assemblies = assemblies);
    }

    public static IConfigurationRoot CreateMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        IConfigurationBuilder configurationBuilder,
        Action<ConfigurationOptions>? action)
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaConfigurationProvider)))
            return new ConfigurationBuilder().Build();

        services.AddSingleton<MasaConfigurationProvider>();
        services.AddOptions();
        services.TryAddSingleton<IMasaConfigurationSourceProvider, DefaultMasaConfigurationSourceProvider>();
        services.TryAddSingleton<IMasaConfiguration, DefaultMasaConfiguration>();
        services.TryAddSingleton<IConfigurationApi, DefaultConfigurationApi>();
        var configurationOptions = new ConfigurationOptions();
        action?.Invoke(configurationOptions);

        var configurationSourceResult = services
            .BuildServiceProvider()
            .GetRequiredService<IMasaConfigurationSourceProvider>()
            .GetMigrated(configurationBuilder, configurationOptions.ExcludeConfigurationSourceTypes,
                configurationOptions.ExcludeConfigurationProviderTypes);

        MasaConfigurationBuilder masaConfigurationBuilder = new MasaConfigurationBuilder(services,
            new ConfigurationBuilder().AddRange(configurationSourceResult.MigrateConfigurationSources));
        configureDelegate?.Invoke(masaConfigurationBuilder);

        MasaConfigurationBuilder builder = new(services, new ConfigurationBuilder());
        builder.AddRelations(masaConfigurationBuilder.Relations.ToArray());
        masaConfigurationBuilder.Repositories.ForEach(repository => builder.AddRepository(repository));
        var localConfigurationRepository = new LocalMasaConfigurationRepository(
            masaConfigurationBuilder.Configuration,
            services.BuildServiceProvider().GetService<ILoggerFactory>());
        builder.AddRepository(localConfigurationRepository);

        var source = new MasaConfigurationSource(builder);
        var configuration = builder
            .Add(source)
            .AddRange(configurationSourceResult.ConfigurationSources)
            .Build();

        builder.AutoMapping(configurationOptions.Assemblies);

        builder.Relations.ForEach(relation =>
        {
            List<string> sectionNames = new()
            {
                relation.SectionType.ToString(),
            };
            if (!string.IsNullOrEmpty(relation.ParentSection))
                sectionNames.Add(relation.ParentSection);

            if (relation.Section != "")
            {
                sectionNames.AddRange(relation.Section!.Split(ConfigurationPath.KeyDelimiter));
            }

            services.ConfigureOption(configuration, sectionNames, relation.ObjectType, relation.Name);
        });

        return configuration;
    }

    private static void ConfigureOption(
        this IServiceCollection services,
        IConfiguration configuration,
        List<string> sectionNames,
        Type optionType,
        string name)
    {
        IConfigurationSection? configurationSection = null;
        foreach (var sectionName in sectionNames)
        {
            if (configurationSection == null)
                configurationSection = configuration.GetSection(sectionName);
            else
                configurationSection = configurationSection.GetSection(sectionName);
        }
        if (!configurationSection.Exists())
            throw new MasaException($"Check if the mapping section is correct，section name is [{configurationSection!.Path}]");

        var configurationChangeTokenSource =
            Activator.CreateInstance(typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionType), name,
                configurationSection)!;
        services.Add(new ServiceDescriptor(typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionType),
            configurationChangeTokenSource));

        Action<BinderOptions> configureBinder = _ =>
        {
        };
        var configureOptions =
            Activator.CreateInstance(typeof(NamedConfigureFromConfigurationOptions<>).MakeGenericType(optionType),
                name,
                configurationSection, configureBinder)!;
        services.Add(new ServiceDescriptor(typeof(IConfigureOptions<>).MakeGenericType(optionType),
            configureOptions));
    }

    private sealed class MasaConfigurationProvider
    {

    }

    private sealed class InitializeAppConfigurationProvider
    {

    }
}
