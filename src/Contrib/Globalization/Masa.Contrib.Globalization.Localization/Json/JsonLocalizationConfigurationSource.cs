// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Globalization.Localization;

public class JsonLocalizationConfigurationSource : IConfigurationSource
{
    public Type ResourceType { get; }

    public string LanguageDirectory { get; }

    public IEnumerable<string> CultureNames { get; }

    public bool UseMasaConfiguration { get; }

    public JsonLocalizationConfigurationSource(
        Type resourceType,
        string languageDirectory,
        IEnumerable<string> cultureNames,
        bool useMasaConfiguration)
    {
        ResourceType = resourceType;
        LanguageDirectory = languageDirectory;
        CultureNames = cultureNames;
        UseMasaConfiguration = useMasaConfiguration;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var configurationProvider = new JsonLocalizationConfigurationProvider(this);
        configurationProvider.Initialize();
        return configurationProvider;
    }
}
