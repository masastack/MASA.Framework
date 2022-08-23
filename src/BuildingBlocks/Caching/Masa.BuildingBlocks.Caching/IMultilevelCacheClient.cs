// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IMultilevelCacheClient : ICacheClient
{
    void Remove<T>(params string[] keys);

    Task RemoveAsync<T>(params string[] keys);

    void Set<T>(string key, T value, CacheEntryOptions<T>? distributedOptions, CacheEntryOptions<T>? memoryOptions);

    Task SetAsync<T>(string key, T value, CacheEntryOptions<T>? distributedOptions, CacheEntryOptions<T>? memoryOptions);

    void Set<T>(string key, T value, CombinedCacheEntryOptions<T>? options);

    Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions<T>? options);

    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? distributedOptions, CacheEntryOptions<T>? memoryOptions);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions<T>? distributedOptions, CacheEntryOptions<T>? memoryOptions);

    void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions<T>? options);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions<T>? options);
}
