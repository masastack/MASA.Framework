// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal class DccConfigurationRepository : AbstractConfigurationRepository
{
    private readonly IConfigurationApiClient _client;

    private readonly IDistributedCacheClient _distributedCacheClient;

    public override SectionTypes SectionType => SectionTypes.ConfigurationApi;

    private readonly ConcurrentDictionary<string, IDictionary<string, string>> _dictionaries = new();

    private readonly ConcurrentDictionary<string, ConfigurationTypes> _configObjectConfigurationTypeRelations = new();

    public DccConfigurationRepository(
        DccSectionOptions defaultSectionOption,
        IEnumerable<DccSectionOptions> expansionSectionOptions,
        IConfigurationApiClient client,
        ILoggerFactory loggerFactory,
        IDistributedCacheClient distributedCacheClient)
        : base(loggerFactory)
    {
        _client = client;
        _distributedCacheClient = distributedCacheClient;

        var sectionOptionOptions = new List<DccSectionOptions>()
        {
            defaultSectionOption
        }.Concat(expansionSectionOptions);

        foreach (var sectionOption in sectionOptionOptions)
        {
            var section = HandleSectionOptions(sectionOption);
            Initialize(section).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    private DccSectionOptions HandleSectionOptions(DccSectionOptions defaultSectionOption)
    {
        if (!defaultSectionOption.ConfigObjects.Any())
        {
            var defaultConfigObjects = new List<string>();

            string partialKey =
                $"{defaultSectionOption.Environment}-{defaultSectionOption.Cluster}-{defaultSectionOption.AppId}".ToLower();
            List<string> keys = _distributedCacheClient.GetKeys($"{partialKey}*");
            foreach (var key in keys)
            {
                var configObject = key.Split($"{partialKey}-", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (configObject == null) continue;

                defaultConfigObjects.Add(configObject);
            }

            defaultSectionOption.ConfigObjects = defaultConfigObjects;
        }

        return defaultSectionOption;
    }

    private async Task Initialize(DccSectionOptions sectionOption)
    {
        foreach (var configObject in sectionOption.ConfigObjects)
        {
            string key = $"{sectionOption.Environment!}-{sectionOption.Cluster!}-{sectionOption.AppId}-{configObject}".ToLower();
            var (Raw, ConfigurationType) = await _client.GetRawAsync(sectionOption.Environment!, sectionOption.Cluster!, sectionOption.AppId, configObject, (val) =>
            {
                if (_configObjectConfigurationTypeRelations.TryGetValue(key, out var configurationType))
                {
                    _dictionaries[key] = FormatRaw(sectionOption.AppId, configObject, val, configurationType);
                    FireRepositoryChange(SectionType, Load());
                }
            });

            _configObjectConfigurationTypeRelations.TryAdd(key, ConfigurationType);
            _dictionaries[key] = FormatRaw(sectionOption.AppId, configObject, Raw, ConfigurationType);
        }
    }

    private static IDictionary<string, string> FormatRaw(string appId, string configObject, string? raw, ConfigurationTypes configurationType)
    {
        if (raw == null)
            return new Dictionary<string, string>();

        return configurationType switch
        {
            ConfigurationTypes.Json => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
            ConfigurationTypes.Properties => SecondaryFormat(appId, configObject, JsonSerializer.Deserialize<Dictionary<string, string>>(raw)!),
            ConfigurationTypes.Text => new Dictionary<string, string>()
                {
                    { $"{appId}{ConfigurationPath.KeyDelimiter}{configObject}" , raw ?? "" }
                },
            ConfigurationTypes.Xml => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
            ConfigurationTypes.Yaml => SecondaryFormat(appId, configObject, JsonConfigurationParser.Parse(raw)),
            _ => throw new NotSupportedException(nameof(configurationType)),
        };
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
