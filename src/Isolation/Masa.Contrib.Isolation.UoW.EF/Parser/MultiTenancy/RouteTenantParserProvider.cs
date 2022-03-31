namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenancy;

public class RouteTenantParserProvider : ITenantParserProvider
{
    public string Name => "Route";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        var tenantId = httpContext?.GetRouteValue(options.Value.TenantKey);
        if (tenantId != null)
        {
            var tenantIdStr = tenantId.ToString();
            tenantSetter.SetTenant(new Tenant(tenantIdStr!));
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
