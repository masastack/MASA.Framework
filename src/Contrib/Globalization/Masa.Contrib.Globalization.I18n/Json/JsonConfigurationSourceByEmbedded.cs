// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Json;

public class JsonConfigurationSourceByEmbedded : IConfigurationSource
{
    public Type ResourceType { get; }

    public string Culture { get; }

    public Stream Stream { get; }

    public bool UseMasaConfiguration { get; }

    public JsonConfigurationSourceByEmbedded(
        Type resourceType,
        Stream stream,
        string culture,
        bool useMasaConfiguration)
    {
        ResourceType = resourceType;
        Stream = stream;
        Culture = culture;
        UseMasaConfiguration = useMasaConfiguration;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var configurationProvider = new JsonConfigurationProviderByEmbedded(this);
        return configurationProvider;
    }
}
