// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache.Tests.Infrastructure;

public class CustomerDistributedCacheClient : BaseDistributedCacheClient
{
    public CustomerDistributedCacheClient(CacheEntryOptions? cacheEntryOptions) : base(cacheEntryOptions)
    {
    }

    public override T? Get<T>(string key) where T : default
    {
        throw new NotImplementedException();
    }

    public override Task<T?> GetAsync<T>(string key) where T : default
    {
        throw new NotImplementedException();
    }

    public override IEnumerable<T?> GetList<T>(IEnumerable<string> keys) where T : default
    {
        throw new NotImplementedException();
    }

    public override Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys) where T : default
    {
        throw new NotImplementedException();
    }

    public override T? Get<T>(string key, Action<T?> valueChanged) where T : default
    {
        throw new NotImplementedException();
    }

    public override Task<T?> GetAsync<T>(string key, Action<T?> valueChanged) where T : default
    {
        throw new NotImplementedException();
    }

    public override T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry) where T : default
    {
        throw new NotImplementedException();
    }

    public override Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry) where T : default
    {
        throw new NotImplementedException();
    }

    public override void Refresh<T>(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public override Task RefreshAsync<T>(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public override void Remove<T>(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public override Task RemoveAsync<T>(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }

    public override void Set<T>(string key, T value, CombinedCacheEntryOptions? options)
    {
        throw new NotImplementedException();
    }

    public override Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options)
    {
        throw new NotImplementedException();
    }

    public override void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options) where T : default
    {
        throw new NotImplementedException();
    }

    public override Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options) where T : default
    {
        throw new NotImplementedException();
    }

    public MemoryCacheEntryOptions? GetBaseMemoryCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
        => base.GetMemoryCacheEntryOptions(cacheEntryOptions);
}
