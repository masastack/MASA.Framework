// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class CacheRelationOptions<TService> : MasaRelationOptions<TService>
    where TService : class
{
    public CacheRelationOptions(string name, Func<IServiceProvider, TService> func) : base(name)
    {
        Func = func;
    }
}
