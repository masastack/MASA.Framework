// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public abstract class CacheClientFactoryBase<TService> : MasaFactoryBase<TService, MasaRelationOptions<TService>>,
    ICacheClientFactory<TService> where TService : class
{
    protected CacheClientFactoryBase(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}
