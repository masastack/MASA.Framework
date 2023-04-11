// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptions<TDbContext> : MasaDbContextOptions
    where TDbContext : DbContext, IMasaDbContext
{
    public MasaDbContextOptions() : base(null, false, typeof(TDbContext), new DbContextOptions<TDbContext>())
    {
    }

    public MasaDbContextOptions(
        IServiceProvider? serviceProvider,
        DbContextOptions originOptions,
        bool enableSoftDelete) : base(serviceProvider, enableSoftDelete, typeof(TDbContext), originOptions)
    {
    }

    private IEnumerable<IModelCreatingProvider>? _modelCreatingProviders;

    /// <summary>
    /// Can be used to filter data
    /// </summary>
    public override IEnumerable<IModelCreatingProvider> ModelCreatingProviders
        => _modelCreatingProviders ??= ServiceProvider?.GetServices<IModelCreatingProvider>() ?? new List<IModelCreatingProvider>();

    private IEnumerable<ISaveChangesFilter<TDbContext>>? _saveChangesFilters;

    /// <summary>
    /// Can be used to intercept SaveChanges(Async) method
    /// </summary>
    public override IEnumerable<ISaveChangesFilter> SaveChangesFilters
        => _saveChangesFilters ??=
            ServiceProvider?.GetServices<ISaveChangesFilter<TDbContext>>() ?? new List<ISaveChangesFilter<TDbContext>>();
}
