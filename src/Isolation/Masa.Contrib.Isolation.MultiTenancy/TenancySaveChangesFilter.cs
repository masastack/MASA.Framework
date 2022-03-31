namespace Masa.Contrib.Isolation.MultiTenancy;

public class TenancySaveChangesFilter<TKey> : ISaveChangesFilter where TKey : IComparable
{
    private readonly ITenantContext _tenantContext;
    private readonly IConvertProvider _convertProvider;

    public TenancySaveChangesFilter(ITenantContext tenantContext, IConvertProvider convertProvider)
    {
        _tenantContext = tenantContext;
        _convertProvider = convertProvider;
    }

    public void OnExecuting(ChangeTracker changeTracker)
    {
        changeTracker.DetectChanges();
        foreach (var entity in changeTracker.Entries().Where(entry => entry.State == EntityState.Added))
        {
            if (entity.Entity is IMultiTenant<TKey>)
            {
                if (_tenantContext.CurrentTenant != null && !string.IsNullOrEmpty(_tenantContext.CurrentTenant.Id))
                {
                    object tenantId = _convertProvider.ChangeType(_tenantContext.CurrentTenant.Id, typeof(TKey));
                    entity.CurrentValues[nameof(IMultiTenant<TKey>.TenantId)] = tenantId;
                }
                else
                {
                    entity.CurrentValues[nameof(IMultiTenant<TKey>.TenantId)] = default(TKey);
                }
            }
        }
    }
}
