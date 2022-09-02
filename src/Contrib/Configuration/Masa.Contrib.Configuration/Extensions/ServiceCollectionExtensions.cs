// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
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
            .GetMigrated(configurationBuilder, configurationOptions.ExcludeConfigurationSourceTypes, configurationOptions.ExcludeConfigurationProviderTypes);

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
}
