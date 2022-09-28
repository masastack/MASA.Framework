﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IMultilevelCacheClient : ICacheClient
{
    T? Get<T>(string key, Action<T?> valueChanged);

    Task<T?> GetAsync<T>(string key, Action<T?> valueChanged);

    T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry);

    Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    void Refresh<T>(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    void Refresh<T>(IEnumerable<string> keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    Task RefreshAsync<T>(params string[] keys);

    /// <summary>
    /// Flush cache time to live
    /// </summary>
    /// <param name="keys"></param>
    Task RefreshAsync<T>(IEnumerable<string> keys);

    void Remove<T>(params string[] keys);

    void Remove<T>(IEnumerable<string> keys);

    Task RemoveAsync<T>(params string[] keys);

    Task RemoveAsync<T>(IEnumerable<string> keys);

    void Set<T>(string key, T value, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions);

    void Set<T>(string key, T value, CombinedCacheEntryOptions? options);

    Task SetAsync<T>(string key, T value, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions);

    Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options);

    void SetList<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions);

    void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CacheEntryOptions? distributedOptions, CacheEntryOptions? memoryOptions);

    Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options);
}
