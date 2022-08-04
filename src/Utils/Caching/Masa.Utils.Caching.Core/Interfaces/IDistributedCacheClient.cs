// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.Interfaces;

/// <summary>
/// The interface for distributed cache client.
/// </summary>
public interface IDistributedCacheClient : ICacheClient
{
    /// <summary>
    /// Refreshes a value's expiration with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    void Refresh(string key);

    /// <summary>
    /// Refreshes a value's expiration with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task RefreshAsync(string key);

    /// <summary>
    /// Checks whether a value exists with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns><see langword="true"/> if the value exists, otherwise <see langword="false"/>.</returns>
    bool Exists<T>(string key);

    /// <summary>
    /// Checks whether a value exists with given key.
    /// </summary>
    /// <param name="key">A string identifying the requested value.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the value <see langword="true"/> if the value exists, otherwise <see langword="false"/>.</returns>
    Task<bool> ExistsAsync<T>(string key);

    /// <summary>
    /// Support fuzzy filtering to obtain key set
    /// </summary>
    /// <param name="keyPattern"></param>
    /// <returns></returns>
    List<string> GetKeys(string keyPattern);

    /// <summary>
    /// Support fuzzy filtering to obtain key set
    /// </summary>
    /// <param name="keyPattern"></param>
    /// <returns></returns>
    Task<List<string>> GetKeysAsync(string keyPattern);

    /// <summary>
    /// Fuzzy query key-value pair collection based on key Pattern
    /// </summary>
    /// <param name="keyPattern"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Dictionary<string, T?> GetListByKeyPattern<T>(string keyPattern);

    /// <summary>
    /// Subscribes to perform some operation when a change to the perferred/active node is broadcast.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on channel.</param>
    void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler);

    /// <summary>
    /// Subscribes to perform some operation when a change to the perferred/active node is broadcast.
    /// </summary>
    /// <param name="channel">The channel to subscribe to.</param>
    /// <param name="handler">The handler to invoke when a message is received on channel.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler);

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="setup">The setup action to configure the <see cref="SubscribeOptions{T}"/>.</param>
    void Publish<T>(string channel, Action<SubscribeOptions<T>> setup);

    /// <summary>
    /// Posts a message to the given channel.
    /// </summary>
    /// <param name="channel">The channel to publish to.</param>
    /// <param name="setup">The setup action to configure the <see cref="SubscribeOptions{T}"/>.</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    Task PublishAsync<T>(string channel, Action<SubscribeOptions<T>> setup);

    /// <summary>
    /// !Destructive support in caching
    /// Increments the number stored at field in the hash stored at key by increment
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The increment number</param>
    /// <returns></returns>
    Task<long> HashIncrementAsync(string key, long value = 1L);

    /// <summary>
    /// !Destructive support in caching
    /// Decrements the number stored at field in the hash stored at key by decrement. return -1 if decrement failed, otherwise return the result.
    /// </summary>
    /// <param name="key">The key</param>
    /// <param name="value">The decrement number</param>
    /// <returns></returns>
    Task<long> HashDecrementAsync(string key, long value = 1L);
}
