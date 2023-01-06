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
    public MultilevelCacheOptions GlobalCacheOptions { get; private set; }

    private static Action<CacheOptions> CacheOptionsAction
        => options => options.CacheKeyType = CacheKeyType.None;

    protected MultilevelCacheClient(ITypeAliasProvider? typeAliasProvider = null)
    {
        _typeAliasProvider = typeAliasProvider;
    }

    public MultilevelCacheClient(
        string name,
        bool isReset,
        IOptionsMonitor<MultilevelCacheGlobalOptions> multilevelCacheGlobalOptions,
        IDistributedCacheClient distributedCacheClient,
        ITypeAliasProvider? typeAliasProvider = null) : this(typeAliasProvider)
    {
        _distributedCacheClient = distributedCacheClient;

        multilevelCacheGlobalOptions.OnChange((option, optionName) =>
        {
            if (name == optionName)
            {
                if (isReset)
                {
                    _memoryCache = new MemoryCache(option);
                }
                _subscribeKeyType = option.SubscribeKeyType;
                _subscribeKeyPrefix = option.SubscribeKeyPrefix;
                GlobalCacheOptions = new MultilevelCacheOptions()
                {
                    CacheKeyType = option.GlobalCacheOptions.CacheKeyType,
                    MemoryCacheEntryOptions = option.CacheEntryOptions
                };
            }
        });

        var options = multilevelCacheGlobalOptions.Get(name) ?? new MultilevelCacheGlobalOptions();
        _memoryCache = new MemoryCache(options);
        _subscribeKeyType = options.SubscribeKeyType;
        _subscribeKeyPrefix = options.SubscribeKeyPrefix;
        GlobalCacheOptions = new MultilevelCacheOptions()
        {
            CacheKeyType = options.GlobalCacheOptions.CacheKeyType,
            MemoryCacheEntryOptions = options.CacheEntryOptions
        };
    }

    public MultilevelCacheClient(IMemoryCache memoryCache,
        IDistributedCacheClient distributedCacheClient,
        MultilevelCacheOptions multilevelCacheOptions,
        SubscribeKeyType subscribeKeyType,
        string subscribeKeyPrefix = "",
        ITypeAliasProvider? typeAliasProvider = null) : this(typeAliasProvider)
    {
        _memoryCache = memoryCache;
        _distributedCacheClient = distributedCacheClient;
        _subscribeKeyType = subscribeKeyType;
        GlobalCacheOptions = multilevelCacheOptions;
        _subscribeKeyPrefix = subscribeKeyPrefix;
    }

    #region Get

    public override T? GetCore<T>(string key,
        Action<T?>? valueChanged,
        Action<MultilevelCacheOptions>? action = null) where T : default
        => GetCoreAsync(key, valueChanged, action,
            (formattedKey, cacheOptionsAction) =>
                Task.FromResult(_distributedCacheClient.Get<T>(formattedKey, cacheOptionsAction))).ConfigureAwait(false).GetAwaiter().GetResult();

    public override Task<T?> GetCoreAsync<T>(string key, Action<T?>? valueChanged, Action<MultilevelCacheOptions>? action = null)
        where T : default
        => GetCoreAsync(key, valueChanged, action,
            (formattedKey, cacheOptionsAction) =>
                _distributedCacheClient.GetAsync<T>(formattedKey, cacheOptionsAction));

    private async Task<T?> GetCoreAsync<T>(string key, Action<T?>? valueChanged,
        Action<MultilevelCacheOptions>? action,
        Func<string, Action<CacheOptions>, Task<T?>> func)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        if (_memoryCache.TryGetValue(formattedKey, out T? value))
            return value;

        value = await func.Invoke(formattedKey, CacheOptionsAction).ConfigureAwait(false);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            Value = value,
            MemoryCacheEntryOptions = GetMemoryCacheEntryOptions(multilevelCacheOptions)
        });

        var channel = FormatSubscribeChannel<T>(key);
        Subscribe(channel, GetSubscribeOptions(valueChanged));

        return value;
    }

    private static SubscribeOptions<T>? GetSubscribeOptions<T>(Action<T?>? valueChanged)
    {
        SubscribeOptions<T>? subscribeOptions = null;
        if (valueChanged != null)
        {
            subscribeOptions = new SubscribeOptions<T>()
            {
                ValueChanged = valueChanged
            };
        }
        return subscribeOptions;
    }

    public override IEnumerable<T?> GetList<T>(
        IEnumerable<string> keys,
        Action<MultilevelCacheOptions>? action = null) where T : default
        => GetListCoreAsync<T>(keys, action,
            (formattedKeys, cacheOptionsAction) =>
                Task.FromResult(_distributedCacheClient.GetList<T>(formattedKeys, cacheOptionsAction))).ConfigureAwait(false).GetAwaiter().GetResult();

    public override Task<IEnumerable<T?>> GetListAsync<T>(
        IEnumerable<string> keys,
        Action<MultilevelCacheOptions>? action = null) where T : default
        => GetListCoreAsync(keys, action,
            (formattedKeys, cacheOptionsAction) =>
                _distributedCacheClient.GetListAsync<T>(formattedKeys, cacheOptionsAction));

    private async Task<IEnumerable<T?>> GetListCoreAsync<T>(
        IEnumerable<string> keys,
        Action<MultilevelCacheOptions>? action,
        Func<IEnumerable<string>, Action<CacheOptions>?, Task<IEnumerable<T?>>> func)
    {
        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var list = GetListCore<T>(FormatCacheKeys<T>(keys, GetCacheKeyType(multilevelCacheOptions)),
            out List<(string Key, string FormattedKey)> awaitCacheKeyItems);

        var awaitValues = new List<T?>();
        if (awaitCacheKeyItems.Any())
            awaitValues = (await func.Invoke(awaitCacheKeyItems.Select(x => x.FormattedKey), CacheOptionsAction).ConfigureAwait(false)).ToList();

        return FillData(list, awaitCacheKeyItems, awaitValues, GetMemoryCacheEntryOptions(multilevelCacheOptions));
    }

    public override T? GetOrSet<T>(string key, CombinedCacheEntry<T> combinedCacheEntry, Action<CacheOptions>? action = null)
        where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = _distributedCacheClient.GetOrSet(formattedKey, combinedCacheEntry.DistributedCacheEntryFunc, CacheOptionsAction);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            PubSub(key, formattedKey, SubscribeOperation.Set, value, combinedCacheEntry.DistributedCacheEntryFunc.Invoke());
        }

        return value;
    }

    public override async Task<T?> GetOrSetAsync<T>(
        string key,
        CombinedCacheEntry<T> combinedCacheEntry,
        Action<CacheOptions>? action = null) where T : default
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        ArgumentNullException.ThrowIfNull(combinedCacheEntry);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        if (!_memoryCache.TryGetValue(formattedKey, out T? value))
        {
            value = await _distributedCacheClient.GetOrSetAsync(
                formattedKey,
                combinedCacheEntry.DistributedCacheEntryFunc,
                CacheOptionsAction).ConfigureAwait(false);

            SetCore(new SetOptions<T>()
            {
                Value = value,
                FormattedKey = formattedKey,
                MemoryCacheEntryOptions = combinedCacheEntry.MemoryCacheEntryOptions
            });

            await PubSubAsync(key, formattedKey, SubscribeOperation.Set, value, combinedCacheEntry.DistributedCacheEntryFunc.Invoke()).ConfigureAwait(false);
        }

        return value;
    }

    #endregion

    #region Set

    public override void Set<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        _distributedCacheClient.Set(formattedKey, value, options?.DistributedCacheEntryOptions, CacheOptionsAction);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        PubSub(key, formattedKey, SubscribeOperation.Set, value, options?.DistributedCacheEntryOptions);
    }

    public override async Task SetAsync<T>(string key, T value, CombinedCacheEntryOptions? options, Action<CacheOptions>? action = null)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(key);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        await _distributedCacheClient.SetAsync(formattedKey, value, options?.DistributedCacheEntryOptions, CacheOptionsAction).ConfigureAwait(false);

        SetCore(new SetOptions<T>()
        {
            FormattedKey = formattedKey,
            MemoryCacheEntryOptions = options?.MemoryCacheEntryOptions,
            Value = value
        });

        await PubSubAsync(key, formattedKey, SubscribeOperation.Set, value, options?.DistributedCacheEntryOptions).ConfigureAwait(false);
    }

    public override void SetList<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKeyValues = FormatKeyValues(keyValues, GetCacheKeyType(multilevelCacheOptions));

        _distributedCacheClient.SetList(formattedKeyValues.ToDictionary(keyValue => keyValue.Key.FormattedKey, keyValue => keyValue.Value),
            options?.DistributedCacheEntryOptions,
            CacheOptionsAction);

        SetListCore(formattedKeyValues, options?.MemoryCacheEntryOptions, item =>
        {
            PubSub(item.Key.Key, item.Key.FormattedKey, SubscribeOperation.Set, item.Value, options?.DistributedCacheEntryOptions);
        });
    }

    public override async Task SetListAsync<T>(
        Dictionary<string, T?> keyValues,
        CombinedCacheEntryOptions? options,
        Action<CacheOptions>? action = null) where T : default
    {
        ArgumentNullException.ThrowIfNull(keyValues);

        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKeyValues = FormatKeyValues(keyValues, GetCacheKeyType(multilevelCacheOptions));

        await _distributedCacheClient.SetListAsync(
            formattedKeyValues.ToDictionary(keyValue => keyValue.Key.FormattedKey, keyValue => keyValue.Value),
            options?.DistributedCacheEntryOptions,
            CacheOptionsAction).ConfigureAwait(false);

        SetListCore(formattedKeyValues, options?.MemoryCacheEntryOptions);

        await Task.WhenAll(formattedKeyValues.Select(item
            => PubSubAsync(item.Key.Key, item.Key.FormattedKey, SubscribeOperation.Set, item.Value,
                options?.DistributedCacheEntryOptions))).ConfigureAwait(false);
    }

    private Dictionary<(string Key, string FormattedKey), T?> FormatKeyValues<T>(
        Dictionary<string, T?> keyValues,
        CacheKeyType cacheKeyType)
    {
        return keyValues.ToDictionary(
            keyValue => (keyValue.Key, FormatCacheKey<T>(keyValue.Key, cacheKeyType)),
            keyValue => keyValue.Value);
    }

    #endregion

    #region Refresh

    public override void Refresh<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var data = FormatCacheKeys<T>(keys, GetCacheKeyType(multilevelCacheOptions));
        var formattedKeys = data.Select(item => item.FormattedKey).ToArray();
        Parallel.ForEach(formattedKeys, key =>
        {
            _memoryCache.TryGetValue(key, out _);
        });
        _distributedCacheClient.Refresh<T>(formattedKeys, CacheOptionsAction);
    }

    public override async Task RefreshAsync<T>(IEnumerable<string> keys, Action<CacheOptions>? action = null)
    {
        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var data = FormatCacheKeys<T>(keys, GetCacheKeyType(multilevelCacheOptions));
        var formattedKeys = data.Select(item => item.FormattedKey).ToArray();
        Parallel.ForEach(formattedKeys, key =>
        {
            _memoryCache.TryGetValue(key, out _);
        });
        await _distributedCacheClient.RefreshAsync<T>(formattedKeys, CacheOptionsAction).ConfigureAwait(false);
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

    private string FormatCacheKey<T>(string key, CacheKeyType cacheKeyType)
        => CacheKeyHelper.FormatCacheKey<T>(
            key,
            cacheKeyType,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName));

    public List<(string Key, string FormattedKey)> FormatCacheKeys<T>(IEnumerable<string> keys, CacheKeyType cacheKeyType)
    {
        return keys.Select(key => (key, CacheKeyHelper.FormatCacheKey<T>(
            key,
            cacheKeyType,
            _typeAliasProvider == null ? null : typeName => _typeAliasProvider.GetAliasName(typeName)))).ToList();
    }

    protected MultilevelCacheOptions GetMultilevelCacheOptions(Action<MultilevelCacheOptions>? action)
    {
        if (action != null)
        {
            var multilevelCacheOptions = new MultilevelCacheOptions();
            action.Invoke(multilevelCacheOptions);
            return multilevelCacheOptions;
        }
        return GlobalCacheOptions;
    }

    private static CacheKeyType GetCacheKeyType(MultilevelCacheOptions multilevelCacheOptions)
        => multilevelCacheOptions.CacheKeyType ?? Constant.DEFAULT_CACHE_KEY_TYPE;

    private CacheEntryOptions? GetMemoryCacheEntryOptions(MultilevelCacheOptions multilevelCacheOptions)
        => multilevelCacheOptions.MemoryCacheEntryOptions ?? GlobalCacheOptions.MemoryCacheEntryOptions;

    private List<CacheItemModel<T>> GetListCore<T>(
        List<(string Key, string FormattedKey)> data,
        out List<(string Key, string MemoryCacheKey)> awaitCacheKeyItems)
    {
        ArgumentNullException.ThrowIfNull(data);

        List<CacheItemModel<T>> list = new();

        foreach (var item in data)
        {
            CacheItemModel<T> cacheItemModel = !_memoryCache.TryGetValue(item.Key, out T? value) ?
                new(item.Key, item.FormattedKey, false, default) :
                new(item.Key, item.FormattedKey, true, value);
            list.Add(cacheItemModel);
        }
        awaitCacheKeyItems = list.Where(x => !x.IsExist)
            .Select(x => (x.Key, x.MemoryCacheKey))
            .Distinct()
            .ToList();
        return list;
    }

    private IEnumerable<T?> FillData<T>(List<CacheItemModel<T>> list,
        List<(string Key, string FormattedKey)> awaitKeys,
        List<T?> awaitValues,
        CacheEntryOptions? memoryCacheEntryOptions)
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
                FormattedKey = cacheKeyItem.FormattedKey,
                Value = value,
                MemoryCacheEntryOptions = memoryCacheEntryOptions
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
        var options = cacheEntryOptions ?? GlobalCacheOptions.MemoryCacheEntryOptions;
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
        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));

        _distributedCacheClient.Remove<T>(formattedKey, CacheOptionsAction);

        PubSub(key, formattedKey, SubscribeOperation.Remove, default(T));

        _memoryCache.Remove(formattedKey);
    }

    private async Task RemoveOneAsync<T>(string key, Action<CacheOptions>? action)
    {
        var multilevelCacheOptions = GetMultilevelCacheOptions(action);
        var formattedKey = FormatCacheKey<T>(key, GetCacheKeyType(multilevelCacheOptions));
        await _distributedCacheClient.RemoveAsync<T>(formattedKey, CacheOptionsAction).ConfigureAwait(false);

        await PubSubAsync(key, formattedKey, SubscribeOperation.Remove, default(T)).ConfigureAwait(false);

        _memoryCache.Remove(formattedKey);
    }

    private void PubSub<T>(
        string key,
        string formattedKey,
        SubscribeOperation operation,
        T? value,
        CacheEntryOptions? cacheEntryOptions = null)
    {
        var channel = FormatSubscribeChannel<T>(key);
        _distributedCacheClient.Publish(channel, subscribeOptions =>
        {
            subscribeOptions.Key = formattedKey;
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = new MultilevelCachePublish<T>(value, cacheEntryOptions);
        });

        if (operation == SubscribeOperation.Remove) _distributedCacheClient.UnSubscribe<T>(channel);
    }

    private async Task PubSubAsync<T>(string key,
        string formattedKey,
        SubscribeOperation operation,
        T? value,
        CacheEntryOptions? cacheEntryOptions = null)
    {
        var channel = FormatSubscribeChannel<T>(key);

        await _distributedCacheClient.PublishAsync(channel, subscribeOptions =>
        {
            subscribeOptions.Key = formattedKey;
            subscribeOptions.Operation = operation;
            subscribeOptions.Value = new MultilevelCachePublish<T>(value, cacheEntryOptions);
        }).ConfigureAwait(false);

        if (operation == SubscribeOperation.Remove) await _distributedCacheClient.UnSubscribeAsync<T>(channel).ConfigureAwait(false);
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

            _distributedCacheClient.Subscribe<MultilevelCachePublish<T>>(channel, subscribeOptions =>
            {
                T? value = default;
                switch (subscribeOptions.Operation)
                {
                    case SubscribeOperation.Set:
                        if (subscribeOptions.Value != null) value = subscribeOptions.Value.Value;
                        SetCore(new SetOptions<T>
                        {
                            FormattedKey = subscribeOptions.Key,
                            Value = value,
                            MemoryCacheEntryOptions = subscribeOptions.Value?.CacheEntryOptions
                        });
                        break;
                    case SubscribeOperation.Remove:
                        _memoryCache.Remove(subscribeOptions.Key);
                        _distributedCacheClient.UnSubscribe<T>(channel);
                        break;
                    default:
                        throw new NotImplementedException();
                }

                options?.ValueChanged?.Invoke(value);
            });

            _subscribeChannels.Add(channel);
        }
    }

    #endregion

}
