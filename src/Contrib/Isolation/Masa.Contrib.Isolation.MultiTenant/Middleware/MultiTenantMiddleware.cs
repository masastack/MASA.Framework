// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.MultiTenant.Middleware;

public class MultiTenantMiddleware : IIsolationMiddleware
{
    private readonly ILogger<MultiTenantMiddleware>? _logger;
    private readonly IEnumerable<IParserProvider> _parserProviders;
    private readonly IMultiTenantContext _tenantContext;
    private readonly IMultiTenantSetter _tenantSetter;
    private readonly IMultiTenantUserContext? _tenantUserContext;
    private readonly string _tenantKey;
    private bool _handled;

    public MultiTenantMiddleware(
        IServiceProvider serviceProvider,
        string tenantKey,
        IEnumerable<IParserProvider>? parserProviders)
    {
        _tenantKey = tenantKey;
        _parserProviders = parserProviders ?? GetDefaultParserProviders();
        _logger = serviceProvider.GetService<ILogger<MultiTenantMiddleware>>();
        _tenantContext = serviceProvider.GetRequiredService<IMultiTenantContext>();
        _tenantSetter = serviceProvider.GetRequiredService<IMultiTenantSetter>();
        _tenantUserContext = serviceProvider.GetService<IMultiTenantUserContext>();
    }

    public async Task HandleAsync(HttpContext? httpContext)
    {
        if (_handled)
            return;

        if (_tenantContext.CurrentTenant != null && !string.IsNullOrEmpty(_tenantContext.CurrentTenant.Id))
        {
            _logger?.LogDebug($"The tenant is successfully resolved, and the resolver is: empty");
            return;
        }

        if (_tenantUserContext is { IsAuthenticated: true, TenantId: { } })
        {
            var tenant = new Tenant(_tenantUserContext.TenantId);
            _tenantSetter.SetTenant(tenant);
            return;
        }

        List<string> parsers = new();
        foreach (var tenantParserProvider in _parserProviders)
        {
            parsers.Add(tenantParserProvider.Name);
            if (await tenantParserProvider.ResolveAsync(httpContext, _tenantKey,
                    tenantId => _tenantSetter.SetTenant(new Tenant(tenantId))))
            {
                _logger?.LogDebug("The tenant is successfully resolved, and the resolver is: {Resolvers}", string.Join("、 ", parsers));
                _handled = true;
                return;
            }
        }
        _logger?.LogDebug("Failed to resolve tenant, and the resolver is: {Resolvers}", string.Join("、 ", parsers));
        _handled = true;
    }

    private List<IParserProvider> GetDefaultParserProviders()
    {
        return new List<IParserProvider>
        {
            new CurrentUserTenantParseProvider(),
            new HttpContextItemParserProvider(),
            new QueryStringParserProvider(),
            new FormParserProvider(),
            new RouteParserProvider(),
            new HeaderParserProvider(),
            new CookieParserProvider()
        };
    }
}
