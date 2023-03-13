// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class MasaConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IDisposable
{
    private readonly ConcurrentDictionary<SectionTypes, Properties> _data;
    private readonly IEnumerable<IConfigurationRepository> _configurationRepositories;
    private readonly List<MigrateConfigurationRelationsInfo> _relations;

    public MasaConfigurationProvider(MasaConfigurationSource source, List<MigrateConfigurationRelationsInfo> relations)
    {
        _data = new();
        _configurationRepositories = source.Builder!.Repositories;
        _relations = relations;

        foreach (var configurationRepository in _configurationRepositories)
        {
            configurationRepository.AddChangeListener(this);
        }
    }

    public override void Load()
    {
        foreach (var configurationRepository in _configurationRepositories)
        {
            var properties = configurationRepository.Load();
            _data[configurationRepository.SectionType] = properties;
        }
        SetData();
    }

    public void OnRepositoryChange(SectionTypes sectionType, Properties newProperties)
    {
        if (_data[sectionType] == newProperties)
            return;

        _data[sectionType] = newProperties;

        SetData();

        OnReload();
    }

    void SetData()
    {
        Dictionary<string, string> data = new(StringComparer.OrdinalIgnoreCase);

        foreach (var configurationType in _data.Keys)
        {
            var properties = _data[configurationType];
            foreach (var key in properties.GetPropertyNames())
            {
                data[$"{configurationType}{ConfigurationPath.KeyDelimiter}{key}"] = properties.GetProperty(key)!;
            }
        }

        Data = MigrateConfiguration(data);
    }

    Dictionary<string, string> MigrateConfiguration(Dictionary<string, string> data)
    {
        Dictionary<string, string> optData = new(data, StringComparer.OrdinalIgnoreCase);
        foreach (var item in _relations)
        {
            var list = data.Where(kvp
                    => kvp.Key.Equals(item.OptSectionName, StringComparison.OrdinalIgnoreCase) ||
                    kvp.Key.StartsWith($"{item.OptSectionName}{ConfigurationPath.KeyDelimiter}"))
                .Select(kvp => new KeyValuePair<string, string>(kvp.Key.Replace(item.OptSectionName, item.SectionName), kvp.Value));
            optData.TryAddRange(list);
        }
        return optData;
    }

    public void Dispose()
    {
        Dispose(true);
        foreach (var configurationRepository in _configurationRepositories)
        {
            configurationRepository.RemoveChangeListener(this);
        }
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
