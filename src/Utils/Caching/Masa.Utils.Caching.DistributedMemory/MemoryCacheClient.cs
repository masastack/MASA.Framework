// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.DistributedMemory;

public class MemoryCacheClient : IMemoryCacheClient
{
    private readonly IMemoryCache _cache;
    private readonly IDistributedCacheClient _distributedClient;

    private readonly SubscribeKeyTypes _subscribeKeyType;
    private readonly string _subscribeKeyPrefix;

    private readonly object _locker = new();
    private readonly IList<string> _subscribeChannels = new List<string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheClient"/> class.
    /// </summary>
    /// <param name="cache">The cache.</param>
    /// <param name="distributedClient">The distributed client.</param>
    /// <param name="subscribeKeyType">The type of subscribe key.</param>
    /// <param name="subscribeKeyPrefix">The prefix of subscribe key.</param>
    public MemoryCacheClient(IMemoryCache cache, IDistributedCacheClient distributedClient, SubscribeKeyTypes subscribeKeyType,
        string subscribeKeyPrefix = "")
    {
        _cache = cache;
        _distributedClient = distributedClient;

        _subscribeKeyType = subscribeKeyType;
        _subscribeKeyPrefix = subscribeKeyPrefix;
    }

    /// <inheritdoc />
    public T? Get<T>(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedClient.Get<T>(key);

            _cache.Set(formattedKey, value);

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe<T>(channel);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedClient.GetAsync<T>(key);

            _cache.Set(formattedKey, value);

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe<T>(channel);
        }

        return value;
    }

    /// <inheritdoc />
    public T? Get<T>(string key, Action<T?> valueChanged)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedClient.Get<T>(key);

            _cache.Set(formattedKey, value);

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new CombinedCacheEntryOptions<T>
            {
                ValueChanged = valueChanged
            });
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedClient.GetAsync<T>(key);

            _cache.Set(formattedKey, value);

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new CombinedCacheEntryOptions<T?>
            {
                ValueChanged = valueChanged
            });
        }

        return value;
    }

    /// <inheritdoc />
    public IEnumerable<T?> GetList<T>(string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        // TODO: whether need to check keys.length

        return keys
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(Get<T>);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<T?>> GetListAsync<T>(string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return
            await Task.WhenAll(keys
                .Where(key => !string.IsNullOrWhiteSpace(key))
                .Select(GetAsync<T>));
    }

    /// <inheritdoc />
    public T? GetOrSet<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T?>? options = null)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (setter == null)
            throw new ArgumentNullException(nameof(setter));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedClient.GetOrSet(key, setter, options);

            if (options == null)
            {
                _cache.Set(formattedKey, value);
            }
            else
            {
                _cache.Set(formattedKey, value, options.MemoryCacheEntryOptions);
            }

            PubSub(key, SubscribeOperation.Set, value, options);
        }

        return value;
    }

    /// <inheritdoc />
    public async Task<T?> GetOrSetAsync<T>(string key, Func<T> setter, CombinedCacheEntryOptions<T>? options = null)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));

        ArgumentNullException.ThrowIfNull(setter, nameof(setter));

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_cache.TryGetValue(key, out T? value))
        {
            value = await _distributedClient.GetOrSetAsync(key, setter, options);

            if (options == null)
            {
                _cache.Set(formattedKey, value);
            }
            else
            {
                _cache.Set(formattedKey, value, options.MemoryCacheEntryOptions);
            }

            await PubSubAsync(key, SubscribeOperation.Set, value, options);
        }

        return value;
    }

    /// <inheritdoc />
    public void Remove<T>(params string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        Parallel.ForEach(keys, RemoveOne<T>);
    }

    /// <inheritdoc />
    public Task RemoveAsync<T>(params string[] keys)
    {
        if (keys == null)
            throw new ArgumentNullException(nameof(keys));

        return Task.WhenAll(keys.Select(RemoveOneAsync<T>));
    }

    /// <inheritdoc />
    public void Set<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        _distributedClient.Set(key, value, options);

        Set(key, value, options?.MemoryCacheEntryOptions);

        PubSub(key, SubscribeOperation.Set, value, options);
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions<T>? options = null)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        await _distributedClient.SetAsync(key, value, options);

        Set(key, value, options?.MemoryCacheEntryOptions);

        await PubSubAsync(key, SubscribeOperation.Set, value, options);
    }

    /// <inheritdoc />
    public void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions<T>? options = null)
    {
        if (keyValues == null)
            throw new ArgumentNullException(nameof(keyValues));

        _distributedClient.SetList(keyValues, options);

        Parallel.ForEach(keyValues, item => PubSub(item.Key, SubscribeOperation.Set, item.Value, options));
    }

    /// <inheritdoc />
    public async Task SetListAsync<T>(Dictionary<string, T> keyValues, CombinedCacheEntryOptions<T>? options = null)
    {
        if (keyValues == null)
            throw new ArgumentNullException(nameof(keyValues));

        await _distributedClient.SetListAsync(keyValues, options);

        await Task.WhenAll(keyValues.Select(item => PubSubAsync(item.Key, SubscribeOperation.Set, item.Value, options)));
    }

    private void RemoveOne<T>(string key)
    {
        _distributedClient.Remove<T>(key);

        Publish<T>(key, SubscribeOperation.Remove);
    }

    private async Task RemoveOneAsync<T>(string key)
    {
        await _distributedClient.RemoveAsync<T>(key);

        await PublishAsync<T>(key, SubscribeOperation.Remove);
    }

    private void Subscribe<T>(string channel, CombinedCacheEntryOptions<T>? options = null)
    {
        if (!_subscribeChannels.Contains(channel))
        {
            lock (_locker)
            {
                if (!_subscribeChannels.Contains(channel))
                {
                    _distributedClient.Subscribe<T>(channel, (subscribeOptions) =>
                    {
                        switch (subscribeOptions.Operation)
                        {
                            case SubscribeOperation.Set:
                                options ??= new CombinedCacheEntryOptions<T>();
                                _cache.Set(subscribeOptions.Key, subscribeOptions.Value, options.MemoryCacheEntryOptions);
                                break;
                            case SubscribeOperation.Remove:
                                _cache.Remove(subscribeOptions.Key);
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        options?.ValueChanged?.Invoke(subscribeOptions.Value);
                    });

                    _subscribeChannels.Add(channel);
                }
            }
        }
    }

    private void Publish<T>(string key, SubscribeOperation operation, T? value = default)
    {
        var channel = FormatSubscribeChannel<T>(key);
        _distributedClient.Publish<T>(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private async Task PublishAsync<T>(string key, SubscribeOperation operation, T? value = default)
    {
        var channel = FormatSubscribeChannel<T>(key);
        await _distributedClient.PublishAsync<T>(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private void PubSub<T>(string key, SubscribeOperation operation, T? value, CombinedCacheEntryOptions<T>? options = null)
    {
        var channel = FormatSubscribeChannel<T>(key);

        if (!options?.IgnoreSubscribe ?? true)
        {
            Subscribe(channel, options);
        }

        _distributedClient.Publish<T>(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private async Task PubSubAsync<T>(string key, SubscribeOperation operation, T? value=default,
        CombinedCacheEntryOptions<T>? options = null)
    {
        var channel = FormatSubscribeChannel<T>(key);

        Subscribe(channel, options);

        await _distributedClient.PublishAsync<T>(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private string FormatMemoryCacheKey<T>(string key) => SubscribeHelper.FormatMemoryCacheKey<T>(key);

    private string FormatSubscribeChannel<T>(string key) =>
        SubscribeHelper.FormatSubscribeChannel<T>(key, _subscribeKeyType, _subscribeKeyPrefix);

    private void Set<T>(string key, T value, MemoryCacheEntryOptions? options = null)
    {
        var formattedKey = FormatMemoryCacheKey<T>(key);
        if (options == null)
        {
            _cache.Set(formattedKey, value);
        }
        else
        {
            _cache.Set(formattedKey, value, options);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _cache.Dispose();
        _distributedClient.Dispose();
    }
}
