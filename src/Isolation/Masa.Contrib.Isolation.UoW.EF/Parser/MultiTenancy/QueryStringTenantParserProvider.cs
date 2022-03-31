namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenancy;

public class QueryStringTenantParserProvider : ITenantParserProvider
{
    public string Name => "QueryString";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        if (httpContext?.Request.Query.ContainsKey(options.Value.TenantKey) ?? false)
        {
            var tenantId = httpContext.Request.Query[options.Value.TenantKey].ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantSetter.SetTenant(new Tenant(tenantId));
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
}
