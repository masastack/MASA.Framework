// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory.Interfaces;

/// <summary>
/// The the interface memory cache client.
/// </summary>
public interface IMemoryCacheClient : ICacheClient
{
    /// <summary>
    /// Gets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <param name="valueChanged">The handler to invoke when the request value changed.</param>
    /// <returns>The located value or null.</returns>
    T? Get<T>(string key, Action<T?> valueChanged);

    /// <summary>
    /// Gets a value with given key.
    /// </summary>
    /// <param name="key">A string identifying the request value.</param>
    /// <param name="valueChanged">The handler to invoke when the request value changed.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous opertion, containing the located value or null.</returns>
    Task<T?> GetAsync<T>(string key, Action<T?> valueChanged);
}
