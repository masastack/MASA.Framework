namespace Masa.Contrib.Isolation.UoW.EF.Parser.MultiTenancy;

public class FormTenantParserProvider : ITenantParserProvider
{
    public string Name => "Form";

    public Task<bool> ResolveAsync(IServiceProvider serviceProvider)
    {
        var httpContext = serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        var tenantSetter = serviceProvider.GetRequiredService<ITenantSetter>();
        var options = serviceProvider.GetRequiredService<IOptionsSnapshot<IsolationOptions>>();
        if (!(httpContext?.Request.HasFormContentType ?? false))
            return Task.FromResult(false);

        if (httpContext?.Request.Form.ContainsKey(options.Value.TenantKey) ?? false)
        {
            var tenantId = httpContext.Request.Form[options.Value.TenantKey].ToString();
            if (!string.IsNullOrEmpty(tenantId))
            {
                tenantSetter.SetTenant(new Tenant(tenantId));
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
}
