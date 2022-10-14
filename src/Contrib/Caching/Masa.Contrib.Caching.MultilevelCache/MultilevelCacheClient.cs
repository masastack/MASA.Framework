// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

public class MultilevelCacheClient : MultilevelCacheClientBase
{
    private readonly ITypeAliasProvider? _typeAliasProvider;
    private IMemoryCache _memoryCache;
    private readonly IDistributedCacheClient _distributedCacheClient;
    private SubscribeKeyType _subscribeKeyType;
    private string _subscribeKeyPrefix;
    private readonly object _locker = new();
    private readonly IList<string> _subscribeChannels = new List<string>();
    public CacheOptions GlobalCacheOptions;
    public CacheEntryOptions? DefaultCacheEntryOptions { get; protected set; }

    private static Action<CacheOptions> CacheOptionsAction
        => options => options.CacheKeyType = CacheKeyType.None;

    protected MultilevelCacheClient(CacheEntryOptions? cacheEntryOptions, ITypeAliasProvider? typeAliasProvider = null)
    {
        DefaultCacheEntryOptions = cacheEntryOptions;
        _typeAliasProvider = typeAliasProvider;
    }

    public MultilevelCacheClient(
        string name,
        bool isReset,
        IOptionsMonitor<MultilevelCacheOptions> multilevelCacheOptions,
        IDistributedCacheClient distributedCacheClient,
        ITypeAliasProvider? typeAliasProvider = null) : this(multilevelCacheOptions.Get(name).CacheEntryOptions, typeAliasProvider)
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
                GlobalCacheOptions = option.GlobalCacheOptions;
            }
        });

        var options = multilevelCacheOptions.Get(name) ?? new MultilevelCacheOptions();
        _memoryCache = new MemoryCache(options);
        _subscribeKeyType = options.SubscribeKeyType;
        _subscribeKeyPrefix = options.SubscribeKeyPrefix;
        GlobalCacheOptions = options.GlobalCacheOptions;
    }

    public MultilevelCacheClient(IMemoryCache memoryCache,
        IDistributedCacheClient distributedCacheClient,
        CacheEntryOptions? cacheEntryOptions,
        SubscribeKeyType subscribeKeyType,
        CacheOptions globalCacheOptions,
        string subscribeKeyPrefix = "",
        ITypeAliasProvider? typeAliasProvider = null) : this(cacheEntryOptions, typeAliasProvider)
    {
        _memoryCache = memoryCache;
        _distributedCacheClient = distributedCacheClient;
        _subscribeKeyType = subscribeKeyType;
        GlobalCacheOptions = globalCacheOptions;
        _subscribeKeyPrefix = subscribeKeyPrefix;
    }

    #region Get

    public override T? Get<T>(string key, Action<CacheOptions>? action = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        if (_memoryCache.TryGetValue(formattedKey, out T? value))
            return value;

        value = _distributedCacheClient.Get<T>(formattedKey, CacheOptionsAction);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            Value = value,
        });

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe<T>(channel);

        return value;
    }

    public override T? Get<T>(string key, Action<T?> valueChanged, Action<CacheOptions>? action = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedCacheClient.Get<T>(formattedKey, CacheOptionsAction);

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

    public override async Task<T?> GetAsync<T>(string key, Action<CacheOptions>? action = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        if (_memoryCache.TryGetValue(formattedKey, out T? value))
            return value;

        value = await _distributedCacheClient.GetAsync<T>(formattedKey, CacheOptionsAction);

        SetCore(new SetOptions<T>
        {
            FormattedKey = formattedKey,
            Value = value,
        });

        var channel = FormatSubscribeChannel<T>(key);

        Subscribe<T>(channel);

        return value;
    }

    public override async Task<T?> GetAsync<T>(string key, Action<T?> valueChanged, Action<CacheOptions>? action = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedCacheClient.GetAsync<T>(formattedKey, CacheOptionsAction);

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

    public override IEnumerable<T?> GetList<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null) where T : default
    {
        var list = GetListCore<T>(FormatCacheKeys<T>(keys, action), out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems);

        List<T?> awaitValues = new();
        if (awaitCacheKeyItems.Count > 0)
        {
            awaitValues = _distributedCacheClient.GetList<T>(awaitCacheKeyItems.Select(x => x.Key), CacheOptionsAction).ToList();
        }

        return FillData(list, awaitCacheKeyItems, awaitValues);
    }

    public override async Task<IEnumerable<T?>> GetListAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
        where T : default
    {
        var list = GetListCore<T>(FormatCacheKeys<T>(keys, action), out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems);

        List<T?> awaitValues = new();
        if (awaitCacheKeyItems.Count > 0)
        {
            awaitValues = (await _distributedCacheClient.GetListAsync<T>(awaitCacheKeyItems.Select(x => x.Key), CacheOptionsAction)).ToList();
        }

        return FillData(list, awaitCacheKeyItems, awaitValues);
    }

    public override T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null)
        where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var formattedKey = FormatCacheKey<T>(key, action);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedCacheClient.GetOrSet(formattedKey, combinedCacheEntry.DistributedCacheEntryFunc, CacheOptionsAction);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            PubSub(key, formattedKey, SubscribeOperation.Set, value);
        }

        return value;
    }

    public override async Task<T?> GetOrSetAsync<T>(
        string key,
        CombinedCacheEntry<T> combinedCacheEntry,
        Action<CacheOptions>? action = null) where T : default
    {
        key.CheckIsNullOrWhiteSpace();

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var formattedKey = FormatCacheKey<T>(key, action);

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedCacheClient.GetOrSetAsync(
                formattedKey,
                combinedCacheEntry.DistributedCacheEntryFunc,
                CacheOptionsAction);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            await PubSubAsync(key, formattedKey, SubscribeOperation.Set, value);
        }

        return value;
    }

    #endregion

    #region Set

    public override void Set<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        _distributedCacheClient.Set(formattedKey, value, options?.DistributedCacheEntryOptions, CacheOptionsAction);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        PubSub(key, formattedKey, SubscribeOperation.Set, value);
    }

    public override async Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
    {
        key.CheckIsNullOrWhiteSpace();

        var formattedKey = FormatCacheKey<T>(key, action);

        await _distributedCacheClient.SetAsync(formattedKey, value, options?.DistributedCacheEntryOptions, CacheOptionsAction);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        await PubSubAsync(key, formattedKey, SubscribeOperation.Set, value);
    }

    public override void SetList<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var formattedKeyValues = FormatKeyValues(keyValues, action);

        _distributedCacheClient.SetList(formattedKeyValues.ToDictionary(keyValue => keyValue.Key.FormattedKey, keyValue => keyValue.Value),
            options?.DistributedCacheEntryOptions,
            CacheOptionsAction);

        SetListCore(formattedKeyValues, options?.MemoryCacheEntryOptions, item =>
        {
            PubSub(item.Key.Key, item.Key.FormattedKey, SubscribeOperation.Set, item.Value);
        });
    }

    public override async Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var formattedKeyValues = FormatKeyValues(keyValues, action);

        await _distributedCacheClient.SetListAsync(
            formattedKeyValues.ToDictionary(keyValue => keyValue.Key.FormattedKey, keyValue => keyValue.Value),
            options?.DistributedCacheEntryOptions,
            CacheOptionsAction);

        SetListCore(formattedKeyValues, options?.MemoryCacheEntryOptions);

        await Task.WhenAll(formattedKeyValues.Select(item
            => PubSubAsync(item.Key.Key, item.Key.FormattedKey, SubscribeOperation.Set, item.Value)));
    }

    private Dictionary<(string Key, string FormattedKey), T?> FormatKeyValues<T>(Dictionary<string, T?> keyValues,
        Action<CacheOptions>? action = null)
    {
        return keyValues.ToDictionary(
            keyValue => (keyValue.Key, FormatCacheKey<T>(keyValue.Key, action)),
            keyValue => keyValue.Value);
    }

    #endregion

    #region Refresh

    public override void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        var formattedKeys = FormatCacheKeys<T>(keys, action);
        Parallel.ForEach(formattedKeys, key =>
        {
            _memoryCache.TryGetValue(key, out _);
        });
        _distributedCacheClient.Refresh<T>(formattedKeys, CacheOptionsAction);
    }

    public override async Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        var formattedKeys = FormatCacheKeys<T>(keys, action);
        Parallel.ForEach(formattedKeys, key =>
        {
            _memoryCache.TryGetValue(key, out _);
        });
        await _distributedCacheClient.RefreshAsync<T>(formattedKeys, CacheOptionsAction);
    }

    #endregion

    #region Remove

    public override void Remove<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        Parallel.ForEach(keys, key => RemoveOne<T>(key, action));
    }

    public override Task RemoveAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        ArgumentNullException.ThrowIfNull(keys);

        return Task.WhenAll(keys.Select(key => RemoveOneAsync<T>(key, action)));
    }

    #endregion

    #region Private methods

    private string FormatCacheKey<T>(string key, Action<CacheOptions>? action)
        => CacheKeyHelper.FormatCacheKey<T>(
            key,
            GetCacheOptions(action).CacheKeyType!.Value,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName));

    private IEnumerable<string> FormatCacheKeys<T>(IEnumerable<string> keys, Action<CacheOptions>? action)
    {
        var cacheKeyType = GetCacheOptions(action).CacheKeyType!.Value;
        return keys.Select(key => CacheKeyHelper.FormatCacheKey<T>(
            key,
            cacheKeyType,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName)));
    }

    protected CacheOptions GetCacheOptions(Action<CacheOptions>? action)
    {
        if (action != null)
        {
            CacheOptions cacheOptions = new CacheOptions();
            action.Invoke(cacheOptions);
            return cacheOptions;
        }
        return GlobalCacheOptions;
    }

    private List<CacheItemModel<T>> GetListCore<T>(
        IEnumerable<string> keys,
        out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems)
    {
        ArgumentNullException.ThrowIfNull(keys);

        List<CacheItemModel<T>> list = new();

        foreach (var key in keys)
        {
            CacheItemModel<T> item = !_memoryCache.TryGetValue(key, out T? value) ?
                new(key, key, false, default) :
                new(key, key, true, value);
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
        string formattedKey = options.FormattedKey!;
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

    private void SetListCore<T>(Dictionary<(string Key, string FormattedKey), T?> keyValues,
        CacheEntryOptions? cacheEntryOptions,
        Action<KeyValuePair<(string Key, string FormattedKey), T>>? action = null)
    {
        var memoryCacheEntryOptions = GetMemoryCacheEntryOptions(cacheEntryOptions);
        foreach (var item in keyValues)
        {
            if (memoryCacheEntryOptions == null)
            {
                _memoryCache.Set(item.Key.FormattedKey, item.Value);
            }
            else
            {
                _memoryCache.Set(item.Key.FormattedKey, item.Value, memoryCacheEntryOptions);
            }
            action?.Invoke(item!);
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

    private void RemoveOne<T>(string key, Action<CacheOptions>? action)
    {
        var formattedKey = FormatCacheKey<T>(key, action);
        _distributedCacheClient.Remove<T>(formattedKey, CacheOptionsAction);

        PubSub(key, formattedKey, SubscribeOperation.Remove, default(T));
    }

    private async Task RemoveOneAsync<T>(string key, Action<CacheOptions>? action)
    {
        var formattedKey = FormatCacheKey<T>(key, action);
        await _distributedCacheClient.RemoveAsync<T>(formattedKey, CacheOptionsAction);

        await PubSubAsync(key, formattedKey, SubscribeOperation.Remove, default(T));
    }

    private void PubSub<T>(
        string key,
        string formattedKey,
        SubscribeOperation operation,
        T? value)
    {
        var channel = FormatSubscribeChannel<T>(key);

        _distributedCacheClient.Publish(channel, subscribeOptions =>
        {
            subscribeOptions.Key = formattedKey;
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = value;
        });
    }

    private async Task PubSubAsync<T>(string key,
        string formattedKey,
        SubscribeOperation operation,
        T? value)
    {
        var channel = FormatSubscribeChannel<T>(key);

        await _distributedCacheClient.PublishAsync(channel, subscribeOptions =>
        {
            subscribeOptions.Key = formattedKey;
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
