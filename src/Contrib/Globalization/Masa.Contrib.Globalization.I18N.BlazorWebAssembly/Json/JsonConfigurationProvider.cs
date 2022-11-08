// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18N.BlazorWebAssembly.Json;

public class JsonConfigurationProvider : ConfigurationProvider
{
    private readonly string _resourceType;
    private readonly bool _useMasaConfiguration;
    private readonly Dictionary<string, Stream> _data;
    private readonly Dictionary<string, Dictionary<string, string>> _dictionary;

    public JsonConfigurationProvider(JsonConfigurationSource configurationSource)
    {
        _resourceType = configurationSource.ResourceType.Name;
        _data = configurationSource.Data;
        _useMasaConfiguration = configurationSource.UseMasaConfiguration;
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
    }

    public override void Load()
    {
        foreach (var item in _data)
        {
            _dictionary[FormatKey(item.Key)] = InitializeConfiguration(item.Value).ConvertToDictionary();
        }

        Data = FormatData();
    }

    private string FormatKey(string cultureName)
    {
        string localSection = BuildingBlocks.Globalization.I18N.Constant.DEFAULT_LOCAL_SECTION;
        var list = _useMasaConfiguration ?
            new List<string>
            {
                SectionTypes.Local.ToString(),
                localSection,
                _resourceType,
                cultureName
            } :
            new List<string>
            {
                localSection,
                _resourceType,
                cultureName
            };
        return string.Join(ConfigurationPath.KeyDelimiter, list);
    }

    private IConfiguration InitializeConfiguration(Stream stream)
    {
        var configurationBuilder = new ConfigurationBuilder();
        var source = new JsonStreamConfigurationSource
        {
            Stream = stream
        };
        configurationBuilder.Sources.Add(source);
        return configurationBuilder.Build();
    }

    private Dictionary<string, string> FormatData()
    {
        var data = new Dictionary<string, string>();
        foreach (var item in _dictionary)
        {
            foreach (var resource in item.Value)
            {
                data[$"{item.Key}{ConfigurationPath.KeyDelimiter}{resource.Key}"] = resource.Value;
            }
        }
        return data;
    }
}
