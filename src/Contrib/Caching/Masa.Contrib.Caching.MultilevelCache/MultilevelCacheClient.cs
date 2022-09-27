// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public class MultilevelCacheClient : MultilevelCacheClientBase
{
    private IMemoryCache _memoryCache;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private SubscribeKeyType _subscribeKeyType;
    private string _subscribeKeyPrefix;
    private readonly object _locker = new();
    private readonly IList<string> _subscribeChannels = new List<string>();

    public CacheEntryOptions? DefaultCacheEntryOptions { get; protected set; }

    protected MultilevelCacheClient(CacheEntryOptions? cacheEntryOptions)
    {
        DefaultCacheEntryOptions = cacheEntryOptions;
    }

    public MultilevelCacheClient(
        string name,
        bool isReset,
        IOptionsMonitor<MultilevelCacheOptions> multilevelCacheOptions,
        IDistributedCacheClient distributedCacheClient) : this(multilevelCacheOptions.Get(name).CacheEntryOptions)
    {
        _distributedCacheClient = distributedCacheClient;

        multilevelCacheOptions.OnChange((option, optionName) =>
        {
            if (name == optionName)
            {
                if (isReset)
                {
                    _memoryCache = new MemoryCache(option);
                }
                _subscribeKeyType = option.SubscribeKeyType;
                _subscribeKeyPrefix = option.SubscribeKeyPrefix;
                DefaultCacheEntryOptions = option.CacheEntryOptions;
            }
        });

        var options = multilevelCacheOptions.Get(name) ?? new MultilevelCacheOptions();
        _memoryCache = new MemoryCache(options);
        _subscribeKeyType = options.SubscribeKeyType;
        _subscribeKeyPrefix = options.SubscribeKeyPrefix;
    }

    public MultilevelCacheClient(IMemoryCache memoryCache,
        IDistributedCacheClient distributedCacheClient,
        CacheEntryOptions? cacheEntryOptions,
        SubscribeKeyType subscribeKeyType,
        string subscribeKeyPrefix = "") : this(cacheEntryOptions)
    {
        _memoryCache = memoryCache;
        _distributedCacheClient = distributedCacheClient;
        _subscribeKeyType = subscribeKeyType;
        _subscribeKeyPrefix = subscribeKeyPrefix;
    }

    #region Get

    public override T? Get<T>(string key) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (_memoryCache.TryGetValue(formattedKey, out T? value))
            return value;

        value = _distributedCacheClient.Get<T>(key);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            Value = value,
        });

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe<T>(channel);

        return value;
    }

    public override T? Get<T>(string key, Action<T?> valueChanged) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedCacheClient.Get<T>(key);

            SetCore(new SetOptions<T>()
            {
                FormattedKey = formattedKey,
                Value = value,
            });

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new SubscribeOptions<T>()
            {
                ValueChanged = valueChanged
            });
        }

        return value;
    }

    public override async Task<T?> GetAsync<T>(string key) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (_memoryCache.TryGetValue(formattedKey, out T? value))
            return value;

        value = await _distributedCacheClient.GetAsync<T>(key);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            Value = value,
        });

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe<T>(channel);

        return value;
    }

    public override async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedCacheClient.GetAsync<T>(key);

            SetCore(new SetOptions<T>()
            {
                FormattedKey = formattedKey,
                Value = value,
            });

            var channel = FormatSubscribeChannel<T>(key);

            Subscribe(channel, new SubscribeOptions<T>()
            {
                ValueChanged = valueChanged
            });
        }

        return value;
    }

    public override IEnumerable<T?> GetList<T>(IEnumerable<string> keys) where T : default
    {
        var list = GetListCore<T>(keys, out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems);

        List<T?> awaitValues = new();
        if (awaitCacheKeyItems.Count > 0)
        {
            awaitValues = _distributedCacheClient.GetList<T>(awaitCacheKeyItems.Select(x => x.Key)).ToList();
        }

        return FillData(list, awaitCacheKeyItems, awaitValues);
    }

    public override async Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys) where T : default
    {
        var list = GetListCore<T>(keys, out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems);

        List<T?> awaitValues = new();
        if (awaitCacheKeyItems.Count > 0)
        {
            awaitValues = (await _distributedCacheClient.GetListAsync<T>(awaitCacheKeyItems.Select(x => x.Key))).ToList();
        }

        return FillData(list, awaitCacheKeyItems, awaitValues);
    }

    public override T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedCacheClient.GetOrSet(key, combinedCacheEntry.DistributedCacheEntryFunc);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            PubSub(key, SubscribeOperation.Set, value);
        }

        return value;
    }

    public override async Task<T?> GetOrSetAsync<T>(string key, CombinedCacheEntry<T> combinedCacheEntry) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var formattedKey = FormatMemoryCacheKey<T>(key);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedCacheClient.GetOrSetAsync(key, combinedCacheEntry.DistributedCacheEntryFunc);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            await PubSubAsync(key, SubscribeOperation.Set, value);
        }

        return value;
    }

    #endregion

    #region Set

    public override void Set<T>(string key, T value, CombinedCacheEntryOptions? options)
    {
        key.CheckIsNullOrWhiteSpace();

        _distributedCacheClient.Set(key, value, options?.DistributedCacheEntryOptions);

        SetCore(new SetOptions<T>()
        {
            Key = key,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        PubSub(key, SubscribeOperation.Set, value);
    }

    public override async Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options)
    {
        key.CheckIsNullOrWhiteSpace();

        await _distributedCacheClient.SetAsync(key, value, options?.DistributedCacheEntryOptions);

        SetCore(new SetOptions<T>()
        {
            Key = key,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        await PubSubAsync(key, SubscribeOperation.Set, value);
    }

    public override void SetList<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        _distributedCacheClient.SetList(keyValues, options?.DistributedCacheEntryOptions);

        SetListCore(keyValues, options?.MemoryCacheEntryOptions, item =>
        {
            PubSub(item.Key, SubscribeOperation.Set, item.Value);
        });
    }

    public override async Task SetListAsync<T>(Dictionary<string, T?> keyValues, CombinedCacheEntryOptions? options) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        await _distributedCacheClient.SetListAsync(keyValues, options?.DistributedCacheEntryOptions);

        SetListCore(keyValues, options?.MemoryCacheEntryOptions);

        await Task.WhenAll(keyValues.Select(item => PubSubAsync(item.Key, SubscribeOperation.Set, item.Value)));
    }

    #endregion

    #region Refresh

    public override void Refresh<T>(IEnumerable<string> keys)
    {
        _distributedCacheClient.Refresh(keys);

        Parallel.ForEach(keys, key =>
        {
            var formattedKey = FormatMemoryCacheKey<T>(key);
            _memoryCache.TryGetValue(formattedKey, out _);
        });
    }

    public override async Task RefreshAsync<T>(IEnumerable<string> keys)
    {
        await _distributedCacheClient.RefreshAsync(keys);

        Parallel.ForEach(keys, key =>
        {
            var formattedKey = FormatMemoryCacheKey<T>(key);
            _memoryCache.TryGetValue(formattedKey, out _);
        });
    }

    #endregion

    #region Remove

    public override void Remove<T>(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        Parallel.ForEach(keys, RemoveOne<T>);
    }

    public override Task RemoveAsync<T>(IEnumerable<string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        return Task.WhenAll(keys.Select(RemoveOneAsync<T>));
    }

    #endregion

    #region Private methods

    protected static string FormatMemoryCacheKey<T>(string key) => SubscribeHelper.FormatMemoryCacheKey<T>(key);

    private List<CacheItemModel<T>> GetListCore<T>(
        IEnumerable<string> keys,
        out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems)
    {
        ArgumentNullException.ThrowIfNull(keys);

        List<CacheItemModel<T>> list = new();

        foreach (var key in keys)
        {
            var formattedKey = FormatMemoryCacheKey<T>(key);
            CacheItemModel<T> item = !_memoryCache.TryGetValue(formattedKey, out T? value) ?
                new(key, formattedKey, false, default) :
                new(key, formattedKey, true, value);
            list.Add(item);
        }
        awaitCacheKeyItems = list.Where(x => !x.IsExist)
            .Select(x => (x.Key, x.MemoryCacheKey))
            .Distinct()
            .ToList();
        return list;
    }

    private IEnumerable<T?> FillData<T>(List<CacheItemModel<T>> list,
        List<(string Key, string MemoryCacheKey)> awaitKeys,
        List<T?> awaitValues)
    {
        for (int index = 0; index < awaitKeys.Count; index++)
        {
            var cacheKeyItem = awaitKeys[index];
            var value = awaitValues[index];
            foreach (var item in list.Where(x => x.Key == cacheKeyItem.Key))
            {
                item.Value = value;
            }

            SetCore(new SetOptions<T>()
            {
                FormattedKey = cacheKeyItem.MemoryCacheKey,
                Value = value,
            });

            var channel = FormatSubscribeChannel<T>(cacheKeyItem.Key);
            Subscribe<T>(channel);
        }

        return list.Select(x => x.Value);
    }

    private void SetCore<T>(SetOptions<T> options)
    {
        string formattedKey = options.FormattedKey ?? FormatMemoryCacheKey<T>(options.Key!);
        var memoryCacheEntryOptions = GetMemoryCacheEntryOptions(options.MemoryCacheEntryOptions);

        if (memoryCacheEntryOptions == null)
        {
            _memoryCache.Set(formattedKey, options.Value);
        }
        else
        {
            _memoryCache.Set(formattedKey, options.Value, memoryCacheEntryOptions);
        }
    }

    private void SetListCore<T>(Dictionary<string, T> keyValues,
        CacheEntryOptions? cacheEntryOptions,
        Action<KeyValuePair<string, T>>? action = null)
    {
        var memoryCacheEntryOptions = GetMemoryCacheEntryOptions(cacheEntryOptions);
        foreach (var item in keyValues)
        {
            string formattedKey = FormatMemoryCacheKey<T>(item.Key);
            if (memoryCacheEntryOptions == null)
            {
                _memoryCache.Set(formattedKey, item.Value);
            }
            else
            {
                _memoryCache.Set(formattedKey, item.Value, memoryCacheEntryOptions);
            }
            action?.Invoke(item);
        }
    }

    protected MemoryCacheEntryOptions? GetMemoryCacheEntryOptions(CacheEntryOptions? cacheEntryOptions)
    {
        var options = cacheEntryOptions ?? DefaultCacheEntryOptions;
        if (options == null)
            return null;

        return CopyTo(options);
    }

    private static MemoryCacheEntryOptions CopyTo(CacheEntryOptions cacheEntryOptions)
    {
        return new()
        {
            AbsoluteExpiration = cacheEntryOptions.AbsoluteExpiration,
            AbsoluteExpirationRelativeToNow = cacheEntryOptions.AbsoluteExpirationRelativeToNow,
            SlidingExpiration = cacheEntryOptions.SlidingExpiration
        };
    }

    private void RemoveOne<T>(string key)
    {
        _distributedCacheClient.Remove(key);

        PubSub(key, SubscribeOperation.Remove, default(T));
    }

    private async Task RemoveOneAsync<T>(string key)
    {
        await _distributedCacheClient.RemoveAsync(key);

        await PubSubAsync(key, SubscribeOperation.Remove, default(T));
    }

    private void PubSub<T>(
        string key,
        SubscribeOperation operation,
        T? value)
    {
        var channel = FormatSubscribeChannel<T>(key);

        _distributedCacheClient.Publish(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private async Task PubSubAsync<T>(string key,
        SubscribeOperation operation,
        T? value)
    {
        var channel = FormatSubscribeChannel<T>(key);

        await _distributedCacheClient.PublishAsync(channel, subscribeOptions =>
        {
            subscribeOptions.Key = FormatMemoryCacheKey<T>(key);
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private string FormatSubscribeChannel<T>(string key) =>
        SubscribeHelper.FormatSubscribeChannel<T>(key, _subscribeKeyType, _subscribeKeyPrefix);

    private void Subscribe<T>(
        string channel,
        SubscribeOptions<T>? options = null)
    {
        if (_subscribeChannels.Contains(channel))
            return;

        lock (_locker)
        {
            if (_subscribeChannels.Contains(channel))
                return;

            _distributedCacheClient.Subscribe<T>(channel, subscribeOptions =>
            {
                switch (subscribeOptions.Operation)
                {
                    case SubscribeOperation.Set:
                        SetCore(new SetOptions<T>()
                        {
                            FormattedKey = subscribeOptions.Key,
                            Value = subscribeOptions.Value,
                        });
                        break;
                    case SubscribeOperation.Remove:
                        _memoryCache.Remove(subscribeOptions.Key);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                options?.ValueChanged?.Invoke(subscribeOptions.Value);
            });

            _subscribeChannels.Add(channel);
        }
    }

    #endregion
}
