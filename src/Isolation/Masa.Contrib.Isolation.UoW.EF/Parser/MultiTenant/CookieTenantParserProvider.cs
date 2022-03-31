namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenant;

public class CookieTenantParserProvider : ITenantParserProvider
{
    public string Name => "Cookie";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        if (httpContext?.Request.Cookies.ContainsKey(options.Value.TenantKey) ?? false)
        {
            var tenantId = httpContext.Request.Cookies[options.Value.TenantKey]!;
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantSetter.SetTenant(new Tenant(tenantId));
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
