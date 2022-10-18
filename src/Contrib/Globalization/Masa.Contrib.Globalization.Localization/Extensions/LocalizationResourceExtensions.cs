// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Globalization.Localization;

public static class LocalizationResourceExtensions
{
    private static IConfiguration? _configuration;

    private static IMasaConfiguration? _masaConfiguration;

    public static LocalizationResource AddJson(
        this LocalizationResource localizationResource,
        string languageDirectory,
        params LanguageInfo[] languages)
    {
        languageDirectory = GetAndCheckLanguageDirectory(languageDirectory);
        var resourceContributors = GetResourceContributors(
            localizationResource,
            languageDirectory,
            languages);
        foreach (var resourceContributor in resourceContributors)
        {
            localizationResource.AddContributor(resourceContributor.CultureName, resourceContributor);
        }

        return localizationResource;
    }

    private static string GetAndCheckLanguageDirectory(string languageDirectory)
    {
        languageDirectory.CheckIsNullOrWhiteSpace();

        if (!Directory.Exists(languageDirectory))
        {
            var path = Path.Combine(LocalizationResourceConfiguration.BaseDirectory, languageDirectory.TrimStart("/"));
            if (!Directory.Exists(path))
            {
                throw new UserFriendlyException($"[{languageDirectory}] does not exist");
            }
            languageDirectory = path;
        }
        else
        {
            languageDirectory = Path.GetFullPath(languageDirectory);
        }
        return languageDirectory;
    }



    private static List<ILocalizationResourceContributor> GetResourceContributors(
        LocalizationResource localizationResource,
        string languageDirectory,
        IEnumerable<LanguageInfo> languages)
    {
        _configuration ??= MasaApp.GetServices().BuildServiceProvider().GetService<IConfiguration>();
        _masaConfiguration ??=
            MasaApp.GetServices().BuildServiceProvider().GetService<IMasaConfiguration>();

        var services = MasaApp.GetServices();

        var configuration = _configuration;

        var useMasaConfiguration = _masaConfiguration != null;
        services.AddJsonLocalizationConfigurationSource(
            languageDirectory,
            languages,
            localizationResource.ResourceType,
            ref configuration,
            useMasaConfiguration);
        _configuration = configuration;

        return languages.Select
            (languageInfo => (ILocalizationResourceContributor)new LocalLocalizationResourceContributor
                (
                    localizationResource.ResourceType,
                    languageInfo.Culture,
                    useMasaConfiguration ? _configuration!.GetSection(SectionTypes.Local.ToString()) : _configuration!
                )
            )
            .ToList();
    }

    private static void AddJsonLocalizationConfigurationSource(
        this IServiceCollection services,
        string languageDirectory,
        IEnumerable<LanguageInfo> languages,
        Type resourceType,
        ref IConfiguration? configuration,
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
            new JsonLocalizationConfigurationSource(resourceType, languageDirectory, languages.Select(l=>l.Culture), useMasaConfiguration);
        configurationBuilder.Add(jsonLocalizationConfigurationSource);
        var localizationConfiguration = configurationBuilder.Build();
        configurationManager.AddConfiguration(localizationConfiguration);
    }
}
