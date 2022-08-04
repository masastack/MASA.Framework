// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public abstract class MasaDbContextOptions : DbContextOptions
{
    public readonly IServiceProvider? ServiceProvider;

    public abstract IEnumerable<IModelCreatingProvider> ModelCreatingProviders { get; }

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public abstract IEnumerable<ISaveChangesFilter> SaveChangesFilters { get; }

    public bool EnableSoftDelete { get; }

    internal MasaDbContextOptions(IServiceProvider? serviceProvider, bool enableSoftDelete)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
    }
}
