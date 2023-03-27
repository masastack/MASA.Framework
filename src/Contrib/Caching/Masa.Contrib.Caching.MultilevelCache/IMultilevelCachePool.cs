// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public interface IMultilevelCachePool : IDisposable
{
    (IMemoryCache MemoryCache, IManualDistributedCacheClient ManualDistributedCacheClient) GetCache(
        IServiceProvider serviceProvider,
        string name,
        MultilevelCacheGlobalOptions multilevelCacheGlobalOptions);

    void TryRemove(string name, MultilevelCacheGlobalOptions multilevelCacheGlobalOptions);
}
