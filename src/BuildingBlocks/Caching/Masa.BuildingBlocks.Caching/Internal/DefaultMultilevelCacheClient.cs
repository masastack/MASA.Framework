// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

internal class DefaultMultilevelCacheClient : IManualMultilevelCacheClient
{
    private readonly IManualMultilevelCacheClient _cacheClient;

    public DefaultMultilevelCacheClient(IManualMultilevelCacheClient cacheClient) => _cacheClient = cacheClient;

    public IEnumerable<T?> GetList<T>(params string[] keys)
        => _cacheClient.GetList<T>(keys);

    public Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys)
        => _cacheClient.GetListAsync<T>(keys);

    public void Set<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, absoluteExpiration, action);

    public void Set<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, absoluteExpirationRelativeToNow, action);

    public void Set<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, options, action);

    public Task SetAsync<T>(string key, T value, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, absoluteExpiration, action);

    public Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, absoluteExpirationRelativeToNow, action);

    public Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, options, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, absoluteExpiration, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan? absoluteExpirationRelativeToNow, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, absoluteExpirationRelativeToNow, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, options, action);

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset? absoluteExpiration, Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, absoluteExpiration, action);

    public Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        TimeSpan? absoluteExpirationRelativeToNow,
        Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, absoluteExpirationRelativeToNow, action);

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null, Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync(keyValues, options, action);

    public void Remove<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.Remove<T>(key, action);

    public void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.Remove<T>(keys, action);

    public Task RemoveAsync<T>(string key, Action<CacheOptions>? action = null)
        => _cacheClient.RemoveAsync<T>(key, action);

    public Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.RemoveAsync<T>(keys, action);

    public void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.Refresh<T>(keys, action);

    public Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        => _cacheClient.RefreshAsync<T>(keys, action);

    public T? Get<T>(string key, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.Get<T>(key, action);

    public T? Get<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.Get(key, valueChanged, action);

    public Task<T?> GetAsync<T>(string key, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.GetAsync<T>(key, action);

    public Task<T?> GetAsync<T>(string key, Action<T?> valueChanged, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.GetAsync(key, valueChanged, action);

    public IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.GetList<T>(keys, action);

    public Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<MultilevelCacheOptions>? action = null)
        => _cacheClient.GetListAsync<T>(keys, action);

    public T? GetOrSet<T>(
        string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        Action<CacheEntryOptions>? memoryCacheEntryOptionsAction = null,
        Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSet(key, distributedCacheEntryFunc, memoryCacheEntryOptionsAction, action);

    public T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSet(key, combinedCacheEntry, action);

    public Task<T?> GetOrSetAsync<T>(
        string key,
        Func<CacheEntry<T>> distributedCacheEntryFunc,
        Action<CacheEntryOptions>? memoryCacheEntryOptionsAction = null,
        Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSetAsync(key, distributedCacheEntryFunc, memoryCacheEntryOptionsAction, action);

    public Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<CacheEntry<T>>> distributedCacheEntryFunc,
        Action<CacheEntryOptions>? memoryCacheEntryOptionsAction = null,
        Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSetAsync(key, distributedCacheEntryFunc, memoryCacheEntryOptionsAction, action);

    public Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null)
        => _cacheClient.GetOrSetAsync(key, combinedCacheEntry, action);

    public void Refresh<T>(params string[] keys)
        => _cacheClient.Refresh<T>(keys);

    public Task RefreshAsync<T>(params string[] keys)
        => _cacheClient.RefreshAsync<T>(keys);

    public void Remove<T>(params string[] keys)
        => _cacheClient.Remove<T>(keys);

    public Task RemoveAsync<T>(params string[] keys)
        => _cacheClient.RemoveAsync<T>(keys);

    public void Set<T>(
        string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, distributedOptions, memoryOptions, action);

    public void Set<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
        => _cacheClient.Set(key, value, options, action);

    public Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, distributedOptions, memoryOptions, action);

    public Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
        => _cacheClient.SetAsync(key, value, options, action);

    public void SetList<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, distributedOptions, memoryOptions, action);

    public void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
        => _cacheClient.SetList(keyValues, options, action);

    public Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CacheEntryOptions? distributedOptions,
        CacheEntryOptions? memoryOptions,
        Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync<T>(keyValues, distributedOptions, memoryOptions, action);

    public Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
        => _cacheClient.SetListAsync<T>(keyValues, options, action);

    public void Dispose()
    {
        //don't need to be released
    }
}
