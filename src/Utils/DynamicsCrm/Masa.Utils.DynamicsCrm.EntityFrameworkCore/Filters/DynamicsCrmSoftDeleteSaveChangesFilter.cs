namespace Masa.Utils.DynamicsCrm.EntityFrameworkCore.Filters;

public class DynamicsCrmSoftDeleteSaveChangesFilter<TDbContext, TUserId> : ISaveChangesFilter<TDbContext>
    where TDbContext : DbContext, IMasaDbContext
    where TUserId : IComparable
{
    private readonly IUserContext? _userContext;
    private readonly ICrmConfiguration? _crmConfiguration;
    private readonly TDbContext _context;

    public DynamicsCrmSoftDeleteSaveChangesFilter(
        TDbContext dbContext,
        IUserContext? userContext = null,
        ICrmConfiguration? crmConfiguration = null)
    {
        _context = dbContext;
        _userContext = userContext;
        _crmConfiguration = crmConfiguration;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();

        var userId = GetUserId();

        var entries = changeTracker.Entries().Where(entry => entry is { State: EntityState.Deleted, Entity: ICrmDeletionAudited });
        foreach (var entity in entries)
        {
            var navigationEntries = entity.Navigations
                .Where(navigationEntry => navigationEntry.Metadata is not ISkipNavigation &&
                                          !((IReadOnlyNavigation)navigationEntry.Metadata).IsOnDependent &&
                                          navigationEntry.CurrentValue != null &&
                                          entries.All(e => e.Entity != navigationEntry.CurrentValue));
            HandleNavigationEntry(navigationEntries);

            entity.State = EntityState.Modified;
            CrmEntityAuditingHelper.SetDeletionAuditProperties(entity.Entity, userId);
        }
    }

    private void HandleNavigationEntry(IEnumerable<NavigationEntry> navigationEntries)
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

    private void HandleDependent(object dependentEntry)
    {
        var userId = _userContext?.GetUserId<Guid>() ?? _crmConfiguration?.SystemUserId ?? Guid.Empty;

        var entityEntry = _context.Entry(dependentEntry);
        entityEntry.State = EntityState.Modified;

        CrmEntityAuditingHelper.SetDeletionAuditProperties(entityEntry.Entity, userId);

        var navigationEntries = entityEntry.Navigations
            .Where(navigationEntry => navigationEntry.Metadata is not ISkipNavigation &&
                                      !((IReadOnlyNavigation)navigationEntry.Metadata).IsOnDependent &&
                                      navigationEntry.CurrentValue != null);
        HandleNavigationEntry(navigationEntries);
    }

    private Guid GetUserId()
    {
        if (_userContext == null)
            return Guid.Empty;

        var userId = _userContext.GetUserId<TUserId>();

        if (userId == null || !(userId is Guid))
            return Guid.Empty;

        return userId as Guid? ?? _crmConfiguration?.SystemUserId ?? Guid.Empty;
    }
}

