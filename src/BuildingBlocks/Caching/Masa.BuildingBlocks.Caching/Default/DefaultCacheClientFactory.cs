// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class DefaultCacheClientFactory<TService> : AbstractMasaFactory<TService, CacheRelationOptions<TService>>,
    ICacheClientFactory<TService> where TService : class
{
    protected DefaultCacheClientFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
