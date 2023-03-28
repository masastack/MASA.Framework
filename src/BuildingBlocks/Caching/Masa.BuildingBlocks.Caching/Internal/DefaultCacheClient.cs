// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Caching;

public class DefaultCacheClient : ICacheClient
{
    private readonly ICacheClient _cacheClient;

    protected DefaultCacheClient(ICacheClient cacheClient) => _cacheClient = cacheClient;

    public IEnumerable<T?> GetList<T>(params string[] keys) => _cacheClient.GetList<T>(keys);

    public Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys) => _cacheClient.GetListAsync<T>(keys);

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
}
