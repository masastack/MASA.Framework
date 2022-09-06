// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface ICacheClient
{
    T? Get<T>(string key);

    Task<T?> GetAsync<T>(string key);

    IEnumerable<T?> GetList<T>(params string[] keys);

    IEnumerable<T?> GetList<T>(IEnumerable<string> keys);

    Task<IEnumerable<T?>> GetListAsync<T>(params string[] keys);

    Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys);

    void Set<T>(string key, T value, DateTimeOffset absoluteExpiration);

    void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    void Set<T>(string key, T value, CacheEntryOptions? options = null);

    Task SetAsync<T>(string key, T value, DateTimeOffset absoluteExpiration);

    Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow);

    Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null);

    void SetList<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration);

    void SetList<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow);

    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, DateTimeOffset absoluteExpiration);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, TimeSpan absoluteExpirationRelativeToNow);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? options = null);
}
