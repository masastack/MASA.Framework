// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public abstract class MasaDbContextOptions : DbContextOptions
{
    public readonly IServiceProvider? ServiceProvider;

    public abstract IEnumerable<IModelCreatingProvider> ModelCreatingProviders { get; }

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public abstract IEnumerable<ISaveChangesFilter> SaveChangesFilters { get; }

    public bool EnableSoftDelete { get; }

    internal Type DbContextType { get; }

    private protected MasaDbContextOptions(IServiceProvider? serviceProvider, bool enableSoftDelete, Type dbContextType)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = enableSoftDelete;
        DbContextType = dbContextType;
    }
}
