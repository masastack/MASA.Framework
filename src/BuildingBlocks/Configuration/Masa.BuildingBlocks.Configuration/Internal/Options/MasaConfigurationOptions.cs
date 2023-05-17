// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Configuration")]
[assembly: InternalsVisibleTo("Masa.Contrib.Configuration.ConfigurationApi.Dcc")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Configuration.Options;

/// <summary>
/// Provider for logging configuration
/// </summary>
internal class MasaConfigurationOptions
{
    private readonly List<(SectionTypes SectionType, Func<IServiceProvider, IConfigurationRepository> Func)> _configurationRepositoryHandlers;

    public IEnumerable<(SectionTypes SectionType, Func<IServiceProvider, IConfigurationRepository> Func)> ConfigurationRepositoryHandlers
        => _configurationRepositoryHandlers;

    public MasaConfigurationOptions() => _configurationRepositoryHandlers = new();

    public void AddConfigurationRepository(
        SectionTypes sectionType,
        Func<IServiceProvider, IConfigurationRepository> configurationRepository)
    {
        if (_configurationRepositoryHandlers.Any(item => item.SectionType == sectionType))
            return;

        _configurationRepositoryHandlers.Add((sectionType, configurationRepository));
    }
}
