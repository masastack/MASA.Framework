// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.Localization;

public static class LocalizationResourceExtensions
{
    private static readonly string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static ILoggerFactory? _loggerFactory;

    public static LocalizationResource AddJson(
        this LocalizationResource localizationResource,
        string languageDirectory,
        JsonSerializerOptions deserializeOptions)
    {
        languageDirectory.CheckIsNullOrWhiteSpace();

        if (!Directory.Exists(languageDirectory))
        {
            var path = Path.Combine(_baseDirectory, languageDirectory);
            if (!Directory.Exists(path))
            {
                throw new UserFriendlyException($"[{languageDirectory}] does not exist");
            }
        }

        var resources = GetResources(localizationResource, languageDirectory, deserializeOptions);
        foreach (var resource in resources)
        {
            localizationResource.AddContributor(resource);
        }

        return localizationResource;
    }

    public static List<ILocalizationResourceContributor> GetResources(
        LocalizationResource localizationResource,
        string languageDirectory,
        JsonSerializerOptions deserializeOptions)
    {
        var files = Directory.GetFiles(languageDirectory).ToList();

        _loggerFactory ??= MasaApp.GetServices().BuildServiceProvider().GetService<ILoggerFactory>();

        return files.Select
            (file => (ILocalizationResourceContributor)new JsonFileLocalizationResourceContributor
                (
                    localizationResource.ResourceType,
                    file,
                    deserializeOptions, _loggerFactory
                )
            )
            .ToList();
    }
}
