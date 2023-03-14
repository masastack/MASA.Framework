// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class MultilevelCacheClientFactoryBase : CacheClientFactoryBase<IManualMultilevelCacheClient>, IMultilevelCacheClientFactory
{
    protected override string DefaultServiceNotFoundMessage
        => "Default MultilevelCache not found, you need to add it, like services.AddMultilevelCache()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] MultilevelCache, it was not found";

    protected override MasaFactoryOptions<CacheRelationOptions<IManualMultilevelCacheClient>> FactoryOptions => _optionsMonitor.CurrentValue;

    private readonly IOptionsMonitor<MultilevelCacheFactoryOptions> _optionsMonitor;

    public MultilevelCacheClientFactoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _optionsMonitor = serviceProvider.GetRequiredService<IOptionsMonitor<MultilevelCacheFactoryOptions>>();
    }
}
