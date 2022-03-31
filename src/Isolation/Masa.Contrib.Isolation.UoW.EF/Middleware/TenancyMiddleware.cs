namespace Masa.Contrib.Isolation.UoW.EF.Middleware;

public class TenancyMiddleware : IIsolationMiddleware
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TenancyMiddleware>? _logger;
    private readonly ITenantContext _tenantContext;
    private readonly IEnumerable<ITenantParserProvider> _tenantParserProviders;
    private bool _handled;
    public TenancyMiddleware(IServiceProvider serviceProvider, IEnumerable<ITenantParserProvider> tenantParserProviders)
    {
        _serviceProvider = serviceProvider;
        _logger = _serviceProvider.GetService<ILogger<TenancyMiddleware>>();
        _tenantContext = _serviceProvider.GetRequiredService<ITenantContext>();
        _tenantParserProviders = tenantParserProviders;
    }

    public async Task HandleAsync()
    {
        if(_handled)
            return;

        if (_tenantContext.CurrentTenant != null && !string.IsNullOrEmpty(_tenantContext.CurrentTenant.Id))
        {
            _logger?.LogDebug($"The tenant is successfully resolved, and the resolver is: empty");
            return;
        }

        List<string> parsers = new();
        foreach (var tenantParserProvider in _tenantParserProviders)
        {
            parsers.Add(tenantParserProvider.Name);
            if (await tenantParserProvider.ResolveAsync(_serviceProvider))
            {
                _logger?.LogDebug($"The tenant is successfully resolved, and the resolver is: {string.Join("、 ", parsers)}");
                _handled = true;
                return;
            }
        }
        _logger?.LogDebug($"Failed to resolve tenant, and the resolver is: {string.Join("、 ", parsers)}");
        _handled = true;
    }
}
