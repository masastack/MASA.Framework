// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Isolation.Parser;

namespace Masa.Contrib.Isolation.MultiTenant.Middleware;

public class MultiTenantMiddleware : IIsolationMiddleware
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MultiTenantMiddleware>? _logger;
    private readonly IEnumerable<IParserProvider> _parserProviders;
    private readonly ITenantContext _tenantContext;
    private readonly ITenantSetter _tenantSetter;
    private readonly string _tenantKey;
    private bool _handled;

    public MultiTenantMiddleware(IServiceProvider serviceProvider, string tenantKey, IEnumerable<IParserProvider>? parserProviders)
    {
        _serviceProvider = serviceProvider;
        _tenantKey = tenantKey;
        _parserProviders = parserProviders ?? GetDefaultParserProviders();
        _logger = _serviceProvider.GetService<ILogger<MultiTenantMiddleware>>();
        _tenantContext = _serviceProvider.GetRequiredService<ITenantContext>();
        _tenantSetter = _serviceProvider.GetRequiredService<ITenantSetter>();
    }

    public async Task HandleAsync()
    {
        if (_handled)
            return;

        if (_tenantContext.CurrentTenant != null && !string.IsNullOrEmpty(_tenantContext.CurrentTenant.Id))
        {
            _logger?.LogDebug($"The tenant is successfully resolved, and the resolver is: empty");
            return;
        }

        List<string> parsers = new();
        foreach (var tenantParserProvider in _parserProviders)
        {
            parsers.Add(tenantParserProvider.Name);
            if (await tenantParserProvider.ResolveAsync(_serviceProvider, _tenantKey, tenantId => _tenantSetter.SetTenant(new Tenant(tenantId))))
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
            new HttpContextItemParserProvider(),
            new QueryStringParserProvider(),
            new FormParserProvider(),
            new RouteParserProvider(),
            new HeaderParserProvider(),
            new CookieParserProvider()
        };
    }
}
