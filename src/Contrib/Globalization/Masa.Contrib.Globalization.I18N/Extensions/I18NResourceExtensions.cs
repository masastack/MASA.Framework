// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Globalization.I18N;

public static class I18NResourceExtensions
{
    private static IConfiguration? _configuration;

    private static IMasaConfiguration? _masaConfiguration;

    public static I18NResource AddJson(
        this I18NResource resource,
        string resourcesDirectory,
        params CultureModel[] supportedCultures)
        => resource.AddJson(resourcesDirectory, supportedCultures.ToList());

    public static I18NResource AddJson(
        this I18NResource resource,
        string resourcesDirectory,
        IEnumerable<CultureModel> supportedCultures)
    {
        if (!resource.Assemblies.Any() && !PathUtils.ParseResourcesDirectory(ref resourcesDirectory))
            return resource;

        var resourceContributors = GetResourceContributors(
            resource,
            resourcesDirectory,
            supportedCultures);
        foreach (var resourceContributor in resourceContributors)
        {
            resource.AddContributor(resourceContributor.CultureName, resourceContributor);
        }
        return resource;
    }

    private static List<II18NResourceContributor> GetResourceContributors(
        I18NResource resource,
        string resourcesDirectory,
        IEnumerable<CultureModel> supportedCultures)
    {
        _configuration ??= MasaApp.GetServices().BuildServiceProvider().GetService<IConfiguration>();
        _masaConfiguration ??=
            MasaApp.GetServices().BuildServiceProvider().GetService<IMasaConfiguration>();

        var services = MasaApp.GetServices();
        var useMasaConfiguration = _masaConfiguration != null;
        var configuration = !resource.Assemblies.Any() ? AddJsonConfigurationSource(
                services,
                resourcesDirectory,
                supportedCultures,
                resource.ResourceType,
                _configuration,
                useMasaConfiguration) :
            AddJsonConfigurationSourceByEmbeddedResource(
                resource.Assemblies,
                services,
                resourcesDirectory,
                supportedCultures,
                resource.ResourceType,
                _configuration,
                useMasaConfiguration);
        _configuration = configuration;

        return supportedCultures.Select(supportedCulture => (II18NResourceContributor)new LocalI18NResourceContributor
                (
                    resource.ResourceType,
                    supportedCulture.Culture,
                    useMasaConfiguration ? _configuration.GetSection(SectionTypes.Local.ToString()) : _configuration!
                )
            )
            .ToList();
    }

    private static IConfiguration AddJsonConfigurationSource(
        IServiceCollection services,
        string resourcesDirectory,
        IEnumerable<CultureModel> supportedCultures,
        Type resourceType,
        IConfiguration? configuration,
        bool useMasaConfiguration)
    {
        return AddJsonConfigurationSourceCore(
            services,
            configuration,
            () => new List<IConfigurationSource>()
            {
                new MasaJsonConfigurationSource(resourceType, resourcesDirectory, supportedCultures.Select(c => c.Culture),
                    useMasaConfiguration)
            });
    }

    private static IConfiguration AddJsonConfigurationSourceByEmbeddedResource(
        IEnumerable<Assembly> assemblies,
        IServiceCollection services,
        string resourcesDirectory,
        IEnumerable<CultureModel> supportedCultures,
        Type resourceType,
        IConfiguration? configuration,
        bool useMasaConfiguration)
    {
        return AddJsonConfigurationSourceCore(services,
            configuration,
            () =>
            {
                var list = new List<IConfigurationSource>();
                var embeddedResourceUtils = new EmbeddedResourceUtils(assemblies);
                var resourceData = embeddedResourceUtils.GetResources(resourcesDirectory);
                foreach (var item in resourceData)
                {
                    foreach (var fileName in item.Value)
                    {
                        var stream = EmbeddedResourceUtils.GetStream(item.Key, fileName);
                        if (stream == null) continue;

                        var culture = EmbeddedResourceUtils.GetCulture(resourcesDirectory, fileName);
                        if (culture != null &&
                            supportedCultures.Any(cul => cul.Culture.Equals(culture, StringComparison.OrdinalIgnoreCase)))
                            list.Add(new JsonConfigurationSourceByEmbedded(resourceType, stream, culture, useMasaConfiguration));
                    }
                }
                return list;
            });
    }

    private static IConfiguration AddJsonConfigurationSourceCore(
        IServiceCollection services,
        IConfiguration? configuration,
        Func<IEnumerable<IConfigurationSource>> func)
    {
        ConfigurationManager configurationManager = new();
        if (configuration == null)
        {
            configuration = configurationManager;
            services.AddSingleton<IConfiguration>(configurationManager);
        }
        else if (configuration is not ConfigurationManager)
        {
            configurationManager.AddConfiguration(configuration);
        }
        else if (configuration is ConfigurationManager configurationManagerTemp)
        {
            configurationManager = configurationManagerTemp;
        }
        var configurationBuilder = new ConfigurationBuilder();

        var jsonLocalizationConfigurationSources = func.Invoke();
        foreach (var source in jsonLocalizationConfigurationSources)
        {
            configurationBuilder.Add(source);
        }

        var localizationConfiguration = configurationBuilder.Build();
        configurationManager.AddConfiguration(localizationConfiguration);
        return configuration;
    }
}
