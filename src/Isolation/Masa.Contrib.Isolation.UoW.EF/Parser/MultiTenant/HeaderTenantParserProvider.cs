namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenant;

public class HeaderTenantParserProvider : ITenantParserProvider
{
    public string Name => "Header";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        if (httpContext?.Request.Headers.ContainsKey(options.Value.TenantKey) ?? false)
        {
            var tenantId = httpContext.Request.Headers[options.Value.TenantKey].ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantSetter.SetTenant(new Tenant(tenantId));
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
