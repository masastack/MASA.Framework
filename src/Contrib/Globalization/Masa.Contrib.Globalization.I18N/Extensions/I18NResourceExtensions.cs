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
        if (!PathUtils.ParseResourcesDirectory(ref resourcesDirectory))
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
        var configuration = AddJsonConfigurationSource(
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
        var jsonLocalizationConfigurationSource =
            new JsonConfigurationSource(resourceType, resourcesDirectory, supportedCultures.Select(c => c.Culture),
                useMasaConfiguration);
        configurationBuilder.Add(jsonLocalizationConfigurationSource);
        var localizationConfiguration = configurationBuilder.Build();
        configurationManager.AddConfiguration(localizationConfiguration);
        return configuration;
    }
}
