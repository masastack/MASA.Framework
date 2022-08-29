// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public class MultilevelCacheClient : BaseDistributedCacheClient
{
    // public T? Get<T>(string key, Action<T?> valueChanged)
    // {
    //     key.CheckIsNullOrWhiteSpace(nameof(key));
    //
    //     var value = Get<T>(key);
    //
    //     if (value == null)
    //         return value;
    //
    //     var channel = FormatSubscribeChannel<T>(key);
    //
    //     Subscribe(channel, new CombinedCacheEntryOptions<T>
    //     {
    //         ValueChanged = valueChanged
    //     });
    //     return value;
    // }
    //
    // public async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged)
    // {
    //     key.CheckIsNullOrWhiteSpace(nameof(key));
    //     var value = await GetAsync<T>(key);
    //
    //     if (value == null)
    //         return value;
    //
    //     var channel = FormatSubscribeChannel<T>(key);
    //
    //     Subscribe(channel, new CombinedCacheEntryOptions<T>
    //     {
    //         ValueChanged = valueChanged
    //     });
    //     return value;
    // }
    //
    // private string FormatSubscribeChannel<T>(string key) =>
    //     SubscribeHelper.FormatSubscribeChannel<T>(key,
    //         _subscribeConfigurationOptions.SubscribeKeyTypes,
    //         _subscribeConfigurationOptions.SubscribeKeyPrefix);
}
