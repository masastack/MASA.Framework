// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EF.Filters;

public class SoftDeleteSaveChangesFilter<TDbContext> : ISaveChangesFilter where TDbContext : DbContext
{
    private readonly TDbContext _context;
    private readonly MasaDbContextOptions<TDbContext> _masaDbContextOptions;

    public SoftDeleteSaveChangesFilter(MasaDbContextOptions<TDbContext> masaDbContextOptions, TDbContext dbContext)
    {
        _masaDbContextOptions = masaDbContextOptions;
        _context = dbContext;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        if (!_masaDbContextOptions.EnableSoftDelete)
            return;

        changeTracker.DetectChanges();
        foreach (var entity in changeTracker.Entries().Where(entry => entry.State == EntityState.Deleted))
        {
            if (entity.Entity is ISoftDelete)
            {
                HandleNavigationEntry(entity.Navigations.Where(n => !((IReadOnlyNavigation)n.Metadata).IsOnDependent));

                entity.State = EntityState.Modified;
                entity.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
            }
        }
    }

    protected virtual void HandleNavigationEntry(IEnumerable<NavigationEntry> navigationEntries)
    {
        foreach (var navigationEntry in navigationEntries)
        {
            if (navigationEntry is CollectionEntry collectionEntry)
            {
                foreach (var dependentEntry in collectionEntry.CurrentValue ?? new List<object>())
                {
                    HandleDependent(dependentEntry);
                }
            }
            else
            {
                var dependentEntry = navigationEntry.CurrentValue;
                if (dependentEntry != null)
                {
                    HandleDependent(dependentEntry);
                }
            }
        }
    }

    protected virtual void HandleDependent(object dependentEntry)
    {
        var entityEntry = _context.Entry(dependentEntry);
        entityEntry.State = EntityState.Modified;

        if (entityEntry.Entity is ISoftDelete)
            entityEntry.CurrentValues[nameof(ISoftDelete.IsDeleted)] = true;
    }
}
