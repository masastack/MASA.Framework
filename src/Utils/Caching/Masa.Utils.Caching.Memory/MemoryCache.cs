// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Memory;

public class MemoryCache<TKey, TValue> : IDisposable where TKey : notnull
{
    private ConcurrentDictionary<TKey, Lazy<TValue>> _dicCache = new();

    public TKey[] Keys
    {
        get
        {
            return _dicCache.Keys.ToArray();
        }
    }

    public TValue[] Values
    {
        get
        {
            return _dicCache.Values.Select(value => value.Value).ToArray();
        }
    }

    public bool Get(TKey key, out TValue? value)
    {
        bool result = _dicCache.TryGetValue(key, out var lazyValue);
        value = lazyValue == null ? default : lazyValue.Value;

        return result;
    }

    public bool TryGet(TKey key, [NotNullWhen(true)]out TValue? value)
    {
        var result = _dicCache.TryGetValue(key, out var lazyValue);

        if (result)
        {
            value =  lazyValue == null ? default :lazyValue.Value;
        }
        else
        {
            value = default;
        }

        return result;
    }

    public bool TryAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        return _dicCache.TryAdd(key, new Lazy<TValue>(() => valueFactory(key), LazyThreadSafetyMode.ExecutionAndPublication));
    }

    public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
    {
        if (!_dicCache.TryGetValue(key, out var lazyValue))
        {
            lazyValue = _dicCache.GetOrAdd(key, k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication));
        }

        return lazyValue.Value;
    }

    /// <summary>
    /// Updates the value associated with key to newValue if the existing value with key is equal to comparisonValue.
    /// </summary>
    /// <param name="key">The key of the value that is compared with comparisonValue and possibly replaced.</param>
    /// <param name="valueFactory">The value that replaces the value of the element that has the specified key if the comparison results in equality.</param>
    /// <param name="comparisonValue">The value that is compared with the value of the element that has the specified key.</param>
    /// <returns>true if the value with key was equal to comparisonValue and was replaced with newValue; otherwise, false.</returns>
    public bool TryUpdate(TKey key, Func<TKey, TValue> valueFactory, TValue comparisonValue)
    {
        return _dicCache.TryUpdate(
            key,
            new Lazy<TValue>(() => valueFactory(key), LazyThreadSafetyMode.ExecutionAndPublication),
            new Lazy<TValue>(() => comparisonValue, LazyThreadSafetyMode.ExecutionAndPublication)
        );
    }

    public TValue AddOrUpdate(TKey key, Func<TKey, TValue> valueFactory)
    {
        return _dicCache.AddOrUpdate
        (
            key,
            k => new Lazy<TValue>(() => valueFactory(k), LazyThreadSafetyMode.ExecutionAndPublication),
            (oldkey, _) => new Lazy<TValue>(() => valueFactory(oldkey), LazyThreadSafetyMode.ExecutionAndPublication)
        ).Value;
    }

    public bool Remove(TKey key)
    {
        return _dicCache.TryRemove(key, out _);
    }

    public bool ContainsKey(TKey key)
    {
        return _dicCache.ContainsKey(key);
    }

    public void Clear()
    {
        _dicCache.Clear();
    }

    public void Dispose()
    {
        _dicCache.Clear();
    }
}
