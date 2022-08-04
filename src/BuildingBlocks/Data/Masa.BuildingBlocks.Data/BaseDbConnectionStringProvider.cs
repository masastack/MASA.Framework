// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public abstract class BaseDbConnectionStringProvider : IDbConnectionStringProvider
{
    private readonly List<MasaDbContextConfigurationOptions>? _dbContextOptionsList = null;

    public virtual List<MasaDbContextConfigurationOptions> DbContextOptionsList => _dbContextOptionsList ?? GetDbContextOptionsList();

    protected abstract List<MasaDbContextConfigurationOptions> GetDbContextOptionsList();
}
