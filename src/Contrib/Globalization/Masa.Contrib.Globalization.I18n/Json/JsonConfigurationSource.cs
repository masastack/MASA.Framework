// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Json;

public class JsonConfigurationSource : IConfigurationSource
{
    public Type ResourceType { get; }

    public string LanguageDirectory { get; }

    public IEnumerable<string> CultureNames { get; }

    public bool UseMasaConfiguration { get; }

    public JsonConfigurationSource(
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
        var configurationProvider = new JsonConfigurationProvider(this);
        configurationProvider.Initialize();
        return configurationProvider;
    }
}
