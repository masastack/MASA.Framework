// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Dispatcher.IntegrationEvents")]
[assembly: InternalsVisibleTo("Masa.Contrib.Isolation")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

internal abstract class LocalMessageDbConnectionStringProviderBase : ILocalMessageDbConnectionStringProvider
{
    private List<MasaDbContextConfigurationOptions>? _dbContextOptionsList = null;

    public virtual List<MasaDbContextConfigurationOptions> DbContextOptionsList => _dbContextOptionsList ??= GetDbContextOptionsList();

    protected abstract List<MasaDbContextConfigurationOptions> GetDbContextOptionsList();
}
