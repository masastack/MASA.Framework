// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Configuration.Options;

public class MasaConfigurationOptions
{
    private readonly List<(SectionTypes SectionType, Func<IServiceProvider, IConfigurationRepository> Func)> _configurationRepositoryHandlers;

    public IReadOnlyList<(SectionTypes SectionType, Func<IServiceProvider, IConfigurationRepository> Func)> ConfigurationRepositoryHandlers
        => _configurationRepositoryHandlers;

    public MasaConfigurationOptions() => _configurationRepositoryHandlers = new();

    public void AddConfigurationRepository(
        SectionTypes sectionType,
        Func<IServiceProvider, IConfigurationRepository> configurationRepository)
    {
        _configurationRepositoryHandlers.Add((sectionType, configurationRepository));
    }
}
