// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration;

public class MasaConfigurationProvider : ConfigurationProvider, IRepositoryChangeListener, IDisposable
{
    private readonly ConcurrentDictionary<SectionTypes, Properties> _data;
    private readonly IEnumerable<IConfigurationRepository> _configurationRepositories;

    public MasaConfigurationProvider(MasaConfigurationSource source)
    {
        _data = new();
        _configurationRepositories = source.Builder!.Repositories;

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

        Data = data;
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
