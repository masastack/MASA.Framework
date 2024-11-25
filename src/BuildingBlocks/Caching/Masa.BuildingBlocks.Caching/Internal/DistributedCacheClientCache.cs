// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

internal class DistributedCacheClientCache
{
    private static ConcurrentDictionary<string, IManualDistributedCacheClient> _cacheClients = new();

    public IManualDistributedCacheClient GetCacheClient(IServiceProvider serviceProvider)
    {
        var environment = GetCurrentEnvironment(serviceProvider);
        var tenantId = GetCurrentTenantId(serviceProvider);

        var key = GenerateKey(environment, tenantId);

        return _cacheClients.GetOrAdd(key, _ => CreateCacheClient(serviceProvider));
    }

    private static string GetCurrentEnvironment(IServiceProvider serviceProvider)
    {
        var multiEnvironmentContext = serviceProvider.GetService<IMultiEnvironmentContext>();
        return multiEnvironmentContext?.CurrentEnvironment ?? string.Empty;
    }

    private static string GetCurrentTenantId(IServiceProvider serviceProvider)
    {
        var multiTenantContext = serviceProvider.GetService<IMultiTenantContext>();
        return multiTenantContext?.CurrentTenant?.Id ?? string.Empty;
    }

    private static string GenerateKey(string environment, string tenantId)
    {
        if (string.IsNullOrEmpty(tenantId))
        {
            return environment;
        }
        return $"{environment}:{tenantId}";
    }

    private static IManualDistributedCacheClient CreateCacheClient(IServiceProvider serviceProvider)
    {
        try
        {
            var scopedService = serviceProvider.GetRequiredService<ScopedService<IManualDistributedCacheClient>>();
            return scopedService.Service;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to create cache client", ex);
        }
    }
}
