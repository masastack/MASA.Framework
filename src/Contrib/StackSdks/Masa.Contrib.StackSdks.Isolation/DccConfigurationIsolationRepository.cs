// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal class DccConfigurationIsolationRepository : AbstractConfigurationRepository
{
    readonly IConfigurationApiClient _client;

    public override SectionTypes SectionType => SectionTypes.ConfigurationApi;

    readonly ConcurrentDictionary<string, IDictionary<string, string?>> _dictionaries = new();

    readonly ConcurrentDictionary<string, ConfigurationTypes> _configObjectConfigurationTypeRelations = new();

    public DccConfigurationIsolationRepository(
        IEnumerable<DccSectionOptions> sectionOptions,
        IConfigurationApiClient client,
        ILoggerFactory loggerFactory)
        : base(loggerFactory)
    {
        _client = client;

        foreach (var sectionOption in sectionOptions)
        {
            Initialize(sectionOption).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }

    private async Task Initialize(DccSectionOptions sectionOption)
    {
        foreach (var configObject in sectionOption.ConfigObjects)
        {
            string key = $"{sectionOption.Environment}-{sectionOption.Cluster}-{sectionOption.AppId}-{configObject}".ToLower();
            var result = await _client.GetRawAsync(sectionOption.Environment, sectionOption.Cluster, sectionOption.AppId, configObject, (val) =>
            {
                if (_configObjectConfigurationTypeRelations.TryGetValue(key, out var configurationType))
                {
                    _dictionaries[key] = FormatRaw(sectionOption.Environment, sectionOption.AppId, configObject, val, configurationType);
                    FireRepositoryChange(SectionType, Load());
                }
            });

            _configObjectConfigurationTypeRelations.TryAdd(key, result.ConfigurationType);
            _dictionaries[key] = FormatRaw(sectionOption.Environment, sectionOption.AppId, configObject, result.Raw, result.ConfigurationType);
        }
    }

    private static IDictionary<string, string?> FormatRaw(string environment, string appId, string configObject, string? raw, ConfigurationTypes configurationType)
    {
        if (raw == null)
            return new Dictionary<string, string?>();

        return configurationType switch
        {
            ConfigurationTypes.Json => SecondaryFormat(environment, appId, configObject, JsonConfigurationParser.Parse(raw)),
            ConfigurationTypes.Properties => SecondaryFormat(environment, appId, configObject, JsonSerializer.Deserialize<Dictionary<string, string>>(raw)!),
            ConfigurationTypes.Text => new Dictionary<string, string?>
        {
            { $"{environment}{ConfigurationPath.KeyDelimiter}{appId}{ConfigurationPath.KeyDelimiter}{configObject}" , raw }
        },
            ConfigurationTypes.Xml => SecondaryFormat(environment, appId, configObject, JsonConfigurationParser.Parse(raw)),
            ConfigurationTypes.Yaml => SecondaryFormat(environment, appId, configObject, JsonConfigurationParser.Parse(raw)),
            _ => throw new NotSupportedException(nameof(configurationType)),
        };
    }

    private static IDictionary<string, string?> SecondaryFormat(
        string environment,
        string appId,
        string configObject,
        IDictionary<string, string?> data)
    {
        var dictionary = new Dictionary<string, string?>();
        foreach (var item in data)
        {
            dictionary[$"{environment}{ConfigurationPath.KeyDelimiter}{appId}{ConfigurationPath.KeyDelimiter}{configObject}{ConfigurationPath.KeyDelimiter}{item.Key}"] = item.Value;
        }
        return dictionary;
    }

    public override Properties Load()
    {
        Dictionary<string, string> properties = _dictionaries
        .SelectMany(item => item.Value)
        .ToDictionary(keyValuePair => keyValuePair.Key, keyValuePair => keyValuePair.Value ?? "");

        return new Properties(properties);
    }
}
