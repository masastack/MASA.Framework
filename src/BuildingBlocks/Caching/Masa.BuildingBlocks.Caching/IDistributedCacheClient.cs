// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IDistributedCacheClient : ICacheClient
{
    void Remove(params string[] keys);

    Task RemoveAsync(params string[] keys);

    bool Exists(string key);

    Task<bool> ExistsAsync(string key);

    List<string> GetKeys(string keyPattern);

    Task<List<string>> GetKeysAsync(string keyPattern);

    List<KeyValuePair<string, T?>> GetListByKeyPattern<T>(string keyPattern);

    Task<List<KeyValuePair<string, T?>>> GetListByKeyPatternAsync<T>(string keyPattern);

    void Publish(string channel, Action<PublishOptions> setup);

    Task PublishAsync(string channel, Action<PublishOptions> setup);

    void Subscribe<T>(string channel, Action<SubscribeOptions<T>> handler);

    Task SubscribeAsync<T>(string channel, Action<SubscribeOptions<T>> handler);

    Task<long> HashIncrementAsync(string key, long value = 1L);

    Task<long> HashDecrementAsync(string key, long value = 1L);

    bool KeyExpire(string key, DateTimeOffset absoluteExpiration);

    Task<bool> KeyExpireAsync(string key, DateTimeOffset absoluteExpiration);

    bool KeyExpire(string key, TimeSpan absoluteExpirationRelativeToNow);

    Task<bool> KeyExpireAsync(string key, TimeSpan absoluteExpirationRelativeToNow);

    bool KeyExpire(string key, CacheEntryOptions? options = null);

    Task<bool> KeyExpireAsync(string key, CacheEntryOptions? options = null);
}
