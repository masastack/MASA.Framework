﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Globalization.I18n.Json;

public class JsonConfigurationProviderByEmbedded : ConfigurationProvider
{
    private readonly string _resourceTypeName;
    private readonly Stream _stream;
    private readonly bool _useMasaConfiguration;
    private Dictionary<string, string> _dictionary;
    private readonly ConfigurationBuilder _configurationBuilder;
    private readonly string _prefix;

    public JsonConfigurationProviderByEmbedded(JsonConfigurationSourceByEmbedded sourceByEmbedded)
    {
        _resourceTypeName = sourceByEmbedded.ResourceType.Name;
        _stream = sourceByEmbedded.Stream;
        _useMasaConfiguration = sourceByEmbedded.UseMasaConfiguration;
        var culture = sourceByEmbedded.Culture;
        _dictionary = new(StringComparer.OrdinalIgnoreCase);
        _configurationBuilder = new ConfigurationBuilder();
        _prefix = FormatKey(culture);
    }

    public override void Load()
    {
        var configuration = new JsonStreamConfigurationSource()
        {
            Stream = _stream
        };
        _configurationBuilder.Add(configuration);
        _dictionary = _configurationBuilder.Build().ConvertToDictionary();

        Data = FormatData();
    }

    private string FormatKey(string cultureName)
    {
        string localSection = BuildingBlocks.Globalization.I18n.Constant.DEFAULT_LOCAL_SECTION;
        var list = _useMasaConfiguration ?
            new List<string>
            {
                SectionTypes.Local.ToString(),
                localSection,
                _resourceTypeName,
                cultureName
            } :
            new List<string>
            {
                localSection,
                _resourceTypeName,
                cultureName
            };
        return string.Join(ConfigurationPath.KeyDelimiter, list);
    }

    private Dictionary<string, string> FormatData()
    {
        var data = new Dictionary<string, string>();
        foreach (var resource in _dictionary)
        {
            data[$"{_prefix}{ConfigurationPath.KeyDelimiter}{resource.Key}"] = resource.Value;
        }
        return data;
    }
}
