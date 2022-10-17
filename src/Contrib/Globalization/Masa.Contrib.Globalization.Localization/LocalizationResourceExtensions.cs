// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public static class LocalizationResourceExtensions
{
    private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static IConfiguration? _configuration;
    private static IMasaConfiguration? _masaConfiguration;

    public static LocalizationResource AddJson(
        this LocalizationResource localizationResource,
        string languageDirectory)
    {
        languageDirectory.CheckIsNullOrWhiteSpace();

        if (!Directory.Exists(languageDirectory))
        {
            var path = Path.Combine(_baseDirectory, languageDirectory.TrimStart("/"));
            if (!Directory.Exists(path))
            {
                throw new UserFriendlyException($"[{languageDirectory}] does not exist");
            }
            languageDirectory = path;
        }

        var resources = GetResources(localizationResource, languageDirectory);
        foreach (var resource in resources)
        {
            localizationResource.AddContributor(resource);
        }

        return localizationResource;
    }

    public static List<ILocalizationResourceContributor> GetResources(
        LocalizationResource localizationResource,
        string languageDirectory)
    {
        var filePaths = Directory.GetFiles(languageDirectory).ToList();

        _configuration ??= MasaApp.GetServices().BuildServiceProvider().GetService<IConfiguration>();
        _masaConfiguration ??= MasaApp.GetServices().BuildServiceProvider().GetService<IMasaConfiguration>();

        var services = MasaApp.GetServices();
        services.AddJsonLocalizationConfigurationSource(ref _configuration, _masaConfiguration, localizationResource.ResourceType, filePaths);

        return filePaths.Select
            (filePath => (ILocalizationResourceContributor)new LocalLocalizationResourceContributor
                (
                    localizationResource.ResourceType,
                    LocalizationResourceConfiguration.Dictionary[filePath],
                    _configuration!,
                    _masaConfiguration
                )
            )
            .ToList();
    }

    private static void AddJsonLocalizationConfigurationSource(
        this IServiceCollection services,
        ref IConfiguration? configuration,
        IMasaConfiguration? masaConfiguration,
        Type resourceType,
        List<string> filePaths)
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
        configurationBuilder.Add(new JsonLocalizationConfigurationSource(resourceType, filePaths, masaConfiguration != null));
        var localizationConfiguration = configurationBuilder.Build();
        configurationManager.AddConfiguration(localizationConfiguration);
    }
}
