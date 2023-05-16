// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class DefaultMultilevelCacheClientFactory : CacheClientFactoryBase<IManualMultilevelCacheClient>, IMultilevelCacheClientFactory
{
    protected override string DefaultServiceNotFoundMessage
        => "Default MultilevelCache not found, you need to add it, like services.AddMultilevelCache()";

    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] MultilevelCache, it was not found";

    protected override MasaFactoryOptions<MasaRelationOptions<IManualMultilevelCacheClient>> FactoryOptions
        => _options.Value;

    private readonly IOptions<MultilevelCacheFactoryOptions> _options;

    public DefaultMultilevelCacheClientFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptions<MultilevelCacheFactoryOptions>>();
    }
}
