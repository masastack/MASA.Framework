namespace MASA.Contrib.Configuration;

public static class ServiceCollectionExtensions
{
    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate = null)
        => builder.AddMasaConfiguration(configureDelegate,
            "Appsettings",
            AppDomain.CurrentDomain.GetAssemblies());

    public static WebApplicationBuilder AddMasaConfiguration(
        this WebApplicationBuilder builder,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        string defaultSectionName = "Appsettings",
        params Assembly[] assemblies)
    {
        var configurationBuilder = GetConfigurationBuilder(builder.Configuration);

        IConfigurationRoot masaConfiguration = builder.Services.CreateMasaConfiguration(configureDelegate, configurationBuilder, defaultSectionName, assemblies);
        if (!masaConfiguration.Providers.Any())
            return builder;

        Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureAppConfiguration(builder.Host, configBuilder =>
        {
            configBuilder.Sources.Clear();
        });
        builder.Configuration.AddConfiguration(masaConfiguration);

        return builder;
    }

    public static IConfigurationRoot CreateMasaConfiguration(
        this IServiceCollection services,
        Action<IMasaConfigurationBuilder>? configureDelegate,
        IConfigurationBuilder? configurationBuilder = null,
        string defaultSectionName = "Appsettings",
        params Assembly[] assemblies)
    {
        if (services.Any(service => service.ImplementationType == typeof(MasaConfigurationProvider)))
            return new ConfigurationBuilder().Build();

        services.AddSingleton<MasaConfigurationProvider>();

        if (!services.Any(service => service.ImplementationType == typeof(ILoggerFactory)))
            services.AddLogging();

        MasaConfigurationBuilder masaConfigurationBuilder = new MasaConfigurationBuilder(new ConfigurationBuilder());
        if (configurationBuilder != null)
        {
            masaConfigurationBuilder.AddSection(configurationBuilder, defaultSectionName);
        }
        configureDelegate?.Invoke(masaConfigurationBuilder);

        if (masaConfigurationBuilder.SectionRelations.Count == 0)
            throw new Exception("Please add the section to be loaded");

        var localConfigurationRepository = new LocalMasaConfigurationRepository(masaConfigurationBuilder.SectionRelations, services.BuildServiceProvider().GetRequiredService<ILoggerFactory>());
        masaConfigurationBuilder.AddRepository(localConfigurationRepository);

        var source = new MasaConfigurationSource(masaConfigurationBuilder);
        var configuration = masaConfigurationBuilder.Add(source).Build();

        masaConfigurationBuilder.AutoMapping(assemblies);
        masaConfigurationBuilder.Relations.ForEach(relation =>
        {
            List<string> sectionNames = new List<string>()
            {
                relation.SectionType.ToString(),
            };
            if (!string.IsNullOrEmpty(relation.ParentSection))
                sectionNames.Add(relation.ParentSection);

            if (relation.Section != "")
            {
                sectionNames.AddRange(relation.Section.Split(ConfigurationPath.KeyDelimiter));
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

    private static void ClearSource(this WebApplicationBuilder builder)
    {
        Microsoft.Extensions.Hosting.HostingHostBuilderExtensions.ConfigureAppConfiguration(builder.Host, configBuilder =>
        {
            configBuilder.Sources.Clear();
        });
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
        {
            throw new ArgumentNullException("Section", "Check if the mapping section is correct");
        }

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
