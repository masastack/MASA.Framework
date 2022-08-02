// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.Interfaces;

/// <summary>
/// The interface for cache client.
/// </summary>
public interface ICacheClient : IDisposable
{
    /// <summary>
    /// Gets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <returns>The located value or null.</returns>
    T? Get<T>(string key);

    /// <summary>
    /// Gets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous opertion, containing the located value or null.</returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Gets or sets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <param name="setter">The setter.</param>
    /// <param name="options">The <see cref="CombinedCacheEntryOptions"/>.</param>
    /// <returns>The located value.</returns>
    T? GetOrSet<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T?>? options = null);

    /// <summary>
    /// Gets or sets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <param name="setter">The setter.</param>
    /// <param name="options">The <see cref="CombinedCacheEntryOptions"/>.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous opertion, containing the located value.</returns>
    Task<T?> GetOrSetAsync<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T>? options = null);

    /// <summary>
    /// Gets a list of values with given keys.
    /// </summary>
    /// <param name="keys">A list of string identifying the request value.</param>
    /// <returns>The located values without <see langword="null"/>.</returns>
    IEnumerable<T?> GetList<T>(string[] keys);

    /// <summary>
    /// Gets a list of values with given keys.
    /// </summary>
    /// <param name="keys">A list of string identifying the request value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous opertion, containing the located values with <see langword="null"/>.</returns>
    Task<IEnumerable<T?>> GetListAsync<T>(string[] keys);

    /// <summary>
    /// Removes a list of values with given keys.
    /// </summary>
    /// <param name="keys">A list of string identifying the requested value.</param>
    void Remove<T>(params string[] keys);

    /// <summary>
    /// Removes a list of values with given keys.
    /// </summary>
    /// <param name="keys">A list of string identifying the requested value.</param>
    Task RemoveAsync<T>(params string[] keys);

    /// <summary>
    /// Sets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set int the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    void Set<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null);

    /// <summary>
    /// Sets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <param name="value">The value to set int the cache.</param>
    /// <param name="options">The cache options for the value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null);

    /// <summary>
    /// Sets a list of cahce items contains key and value.
    /// </summary>
    /// <param name="keyValues">The <see cref="Dictionary{TKey, TValue}"/> contains the cache key and cache value.</param>
    /// <param name="options">The cache options for the value.</param>
    void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions<T>? options = null);

    /// <summary>
    /// Sets a list of cahce items contains key and value.
    /// </summary>
    /// <param name="keyValues">The <see cref="Dictionary{TKey, TValue}"/> contains the cache key and cache value.</param>
    /// <param name="options">The cache options for the value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SetListAsync<T>(Dictionary<string, T> keyValues, CombinedCacheEntryOptions<T>? options = null);
}
