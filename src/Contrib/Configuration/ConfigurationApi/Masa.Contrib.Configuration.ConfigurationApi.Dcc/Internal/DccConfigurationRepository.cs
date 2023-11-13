// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Google.Protobuf.WellKnownTypes;
using Masa.BuildingBlocks.Configuration;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Diagnostics.Metrics;
using System;
using YamlDotNet.Core.Tokens;

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal class DccConfigurationRepository : AbstractConfigurationRepository
{
    private readonly IConfigurationApiClient _client;

    public override SectionTypes SectionType => SectionTypes.ConfigurationApi;

    private readonly ConcurrentDictionary<string, IDictionary<string, string>> _dictionaries = new();

    private readonly ConcurrentDictionary<string, ConfigurationTypes> _configObjectConfigurationTypeRelations = new();

    public DccConfigurationRepository(
        IEnumerable<DccSectionOptions> sectionOptions,
        IConfigurationApiClient client,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _client = client;

        Initialize(sectionOptions).ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task Initialize(IEnumerable<DccSectionOptions> sectionOptions)
    {
        foreach (var sectionOption in sectionOptions.Where(opt => opt.AppId != DEFAULT_PUBLIC_ID))
        {
            var result = await _client.GetRawsAsync(sectionOption.Environment, sectionOption.Cluster, sectionOption.AppId);

            foreach (var item in result)
            {
                string key = item.ConfigObject;
                string appId = sectionOption.AppId.ToLower();
                string environmentClusterAppIdName = $"{sectionOption.Environment}-{sectionOption.Cluster}-{appId}-".ToLower();
                string configObject = item.ConfigObject.Replace(environmentClusterAppIdName, string.Empty);

                if (item.ConfigObject.Contains(DEFAULT_PUBLIC_PUBLIC))
                {
                    appId = DEFAULT_PUBLIC_ID;
                }
                if (_configObjectConfigurationTypeRelations.TryGetValue(key, out var configurationType))
                {
                    _dictionaries[key] = FormatRaw(appId, configObject, item.Raw, configurationType);
                    FireRepositoryChange(SectionType, Load());
                }

                _dictionaries[key] = FormatRaw(appId, configObject, item.Raw, item.ConfigurationType);
                sectionOption.ConfigObjects.Add(configObject);
            }
        }
    }

    private static IDictionary<string, string> FormatRaw(string appId, string configObject, string? raw, ConfigurationTypes configurationType)
    {
        try
        {
            if (raw == null)
                return new Dictionary<string, string>();

            return configurationType switch
            {
                ConfigurationTypes.Json => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
                ConfigurationTypes.Properties => SecondaryFormat(appId, configObject, JsonSerializer.Deserialize<Dictionary<string, string>>(raw)!),
                ConfigurationTypes.Text => new Dictionary<string, string>
            {
                    { $"{appId}{ConfigurationPath.KeyDelimiter}{configObject}" , raw }
                },
                ConfigurationTypes.Xml => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
                ConfigurationTypes.Yaml => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
                _ => throw new NotSupportedException(nameof(configurationType)),
            };

        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private static IDictionary<string, string> SecondaryFormat(
        string appId,
        string configObject,
        IDictionary<string, string> data)
    {
        var dictionary = new Dictionary<string, string>();
        foreach (var item in data)
        {
            dictionary[$"{appId}{ConfigurationPath.KeyDelimiter}{configObject}{ConfigurationPath.KeyDelimiter}{item.Key}"] = item.Value;
        }
        return dictionary;
    }

    public override Properties Load()
    {
        Dictionary<string, string> properties = new();
        foreach (var item in _dictionaries)
        {
            foreach (var key in item.Value.Keys)
            {
                properties[key] = item.Value[key];
            }
        }
        return new Properties(properties);
    }

}
