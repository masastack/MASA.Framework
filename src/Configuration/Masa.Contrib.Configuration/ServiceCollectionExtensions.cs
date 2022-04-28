// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate = null)
        => builder.AddMasaConfiguration(configureDelegate,
            AppDomain.CurrentDomain.GetAssemblies());

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        params Assembly[] assemblies)
        => builder.AddMasaConfiguration(null, assemblies);

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        params Assembly[] assemblies)
    {
        var configurationBuilder = GetConfigurationBuilder(builder.Configuration);

        IConfigurationRoot masaConfiguration = builder.Services.CreateMasaConfiguration(configureDelegate, configurationBuilder, assemblies);
        if (!masaConfiguration.Providers.Any())
            return builder;

        Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureAppConfiguration(builder.Host, configBuilder => configBuilder.Sources.Clear());
        builder.Configuration.AddConfiguration(masaConfiguration);

        return builder;
    }

    public static IConfigurationRoot CreateMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        IConfigurationBuilder configurationBuilder,
        params Assembly[] assemblies)
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaConfigurationProvider)))
            return new ConfigurationBuilder().Build();

        services.AddSingleton<MasaConfigurationProvider>();
        services.AddOptions();
        services.TryAddSingleton<IMasaConfiguration, DefaultMasaConfiguration>();

        MasaConfigurationBuilder masaConfigurationBuilder = new MasaConfigurationBuilder(services, configurationBuilder);
        configureDelegate?.Invoke(masaConfigurationBuilder);

        MasaConfigurationBuilder builder = new(services, new ConfigurationBuilder());
        builder.AddRelations(masaConfigurationBuilder.Relations.ToArray());
        masaConfigurationBuilder.Repositories.ForEach(repository => builder.AddRepository(repository));
        var localConfigurationRepository = new LocalMasaConfigurationRepository(masaConfigurationBuilder.Configuration, services.BuildServiceProvider().GetService<ILoggerFactory>());
        builder.AddRepository(localConfigurationRepository);

        var source = new MasaConfigurationSource(builder);
        var configuration = builder.Add(source).Build();

        builder.AutoMapping(assemblies);

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

            services.ConfigureOption(configuration, sectionNames, relation.ObjectType);
        });

        return configuration;
    }

    private static IConfigurationBuilder GetConfigurationBuilder(ConfigurationManager configuration)
    {
        var configurationBuilder = new ConfigurationBuilder();
        foreach (var source in ((IConfigurationBuilder)configuration).Sources)
        {
            configurationBuilder.Add(source);
        }
        return configurationBuilder;
    }

    internal static void ConfigureOption(
        this IServiceCollection services,
        IConfiguration configuration,
        List<string> sectionNames, Type optionType)
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
            throw new Exception($"Check if the mapping section is correct，section name is [{configurationSection!.Path}]");

        var configurationChangeTokenSource =
            Activator.CreateInstance(typeof(ConfigurationChangeTokenSource<>).MakeGenericType(optionType), string.Empty,
                configurationSection)!;
        services.TryAdd(new ServiceDescriptor(typeof(IOptionsChangeTokenSource<>).MakeGenericType(optionType),
            configurationChangeTokenSource));

        Action<BinderOptions> configureBinder = _ => { };
        var configureOptions =
            Activator.CreateInstance(typeof(NamedConfigureFromConfigurationOptions<>).MakeGenericType(optionType),
                string.Empty,
                configurationSection, configureBinder)!;
        services.TryAdd(new ServiceDescriptor(typeof(IConfigureOptions<>).MakeGenericType(optionType),
            configureOptions));
    }

    private class MasaConfigurationProvider
    {

    }
}
