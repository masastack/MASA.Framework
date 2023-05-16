// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class DefaultDistributedCacheClientFactory : CacheClientFactoryBase<IManualDistributedCacheClient>, IDistributedCacheClientFactory
{
    protected override string DefaultServiceNotFoundMessage
        => "Default DistributedCache not found, you need to add it, like services.AddStackExchangeRedisCache()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] DistributedCache, it was not found";

    protected override MasaFactoryOptions<MasaRelationOptions<IManualDistributedCacheClient>> FactoryOptions => _options.Value;

    private readonly IOptions<DistributedCacheFactoryOptions> _options;

    public DefaultDistributedCacheClientFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptions<DistributedCacheFactoryOptions>>();
    }
}
