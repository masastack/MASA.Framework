// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Isolation;

internal class MasaConfigurationIsolationProvider : ConfigurationProvider, IRepositoryChangeListener, IDisposable
{
    private readonly ConcurrentDictionary<SectionTypes, Properties> _data;
    private readonly DccConfigurationIsolationRepository _configurationIsolationRepository;

    public MasaConfigurationIsolationProvider(DccConfigurationIsolationRepository configurationIsolationRepository)
    {
        _data = new();
        _configurationIsolationRepository = configurationIsolationRepository;
        configurationIsolationRepository.AddChangeListener(this);
    }

    public override void Load()
    {
        var properties = _configurationIsolationRepository.Load();
        _data[_configurationIsolationRepository.SectionType] = properties;
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
        Data = _data.SelectMany(kv =>
            kv.Value.GetPropertyNames().Select(key =>
                new KeyValuePair<string, string>($"{kv.Key}{ConfigurationPath.KeyDelimiter}{key}", kv.Value.GetProperty(key)!)
            )
        ).ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _configurationIsolationRepository.RemoveChangeListener(this);
    }
}
