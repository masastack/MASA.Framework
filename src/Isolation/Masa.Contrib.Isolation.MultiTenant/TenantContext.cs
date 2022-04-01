namespace Masa.Contrib.Isolation.MultiTenant;

public class TenantContext : ITenantContext, ITenantSetter
{
    public Tenant? CurrentTenant { get; private set; }

    public void SetTenant(Tenant? tenant) => CurrentTenant = tenant;
}
