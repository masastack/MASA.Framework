// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly.Json;

public class JsonConfigurationSource : IConfigurationSource
{
    public Type ResourceType { get; }

    public Dictionary<string, Stream> Data { get; }

    public bool UseMasaConfiguration { get; }

    public JsonConfigurationSource(
        Type resourceType,
        Dictionary<string, Stream> data,
        bool useMasaConfiguration)
    {
        ResourceType = resourceType;
        Data = data;
        UseMasaConfiguration = useMasaConfiguration;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var configurationProvider = new JsonConfigurationProvider(this);
        return configurationProvider;
    }
}
