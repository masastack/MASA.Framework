namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenancy;

public class HttpContextItemTenantParserProvider : ITenantParserProvider
{
    public string Name => "Items";

    public Task<bool> ExecuteAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        if (httpContext?.Items.ContainsKey(options.Value.TenantKey) ?? false)
        {
            var tenantId = httpContext.Items[options.Value.TenantKey]?.ToString() ?? string.Empty;
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantSetter.SetTenant(new Tenant(tenantId));
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }
}
