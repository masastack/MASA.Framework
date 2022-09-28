// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

[Obsolete("BaseDbConnectionStringProvider has expired, please use DbConnectionStringProviderBase")]
public abstract class BaseDbConnectionStringProvider : DbConnectionStringProviderBase
{
}

public abstract class DbConnectionStringProviderBase : IDbConnectionStringProvider
{
    private readonly List<MasaDbContextConfigurationOptions>? _dbContextOptionsList = null;

    public virtual List<MasaDbContextConfigurationOptions> DbContextOptionsList => _dbContextOptionsList ?? GetDbContextOptionsList();

    protected abstract List<MasaDbContextConfigurationOptions> GetDbContextOptionsList();
}
