// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface ICacheClient
{
    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    T? Get<T>(string key, Action<T?> valueChanged);

    Task<T?> GetAsync<T>(string key, Action<T?> valueChanged);

    IEnumerable<T?> GetList<T>(params string[] keys);

    Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys);

    T? GetOrSet<T>(string key, Func<CacheEntry<T>> setter);

    Task<T?> GetOrSetAsync<T>(string key, Func<CacheEntry<T>> setter);

    void Set<T>(string key, T value, DateTimeOffset absoluteExpiration);

    Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration);

    void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    void Set<T>(string key, T value, CacheEntryOptions<T>? options = null);

    Task SetAsync<T>(string key, T value, CacheEntryOptions<T>? options = null);

    void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration);

    void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow);

    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? options = null);

    void Refresh(string key);

    Task RefreshAsync(string key);
}
