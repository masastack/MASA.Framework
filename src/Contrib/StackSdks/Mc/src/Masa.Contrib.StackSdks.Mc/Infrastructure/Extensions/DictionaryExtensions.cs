// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Mc.Infrastructure.Extensions;

/// <summary>
/// Extension methods for Dictionary.
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// This method is used to try to get a value in a dictionary if it does exists.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="dictionary">The collection object</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value of the key (or default value if key not exists)</param>
    /// <returns>True if key does exists in the dictionary</returns>
    internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
    {
        object valueObj;
        if (dictionary.TryGetValue(key, out valueObj) && valueObj is T)
        {
            value = (T)valueObj;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue? GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, Func<TKey, TValue> factory)
    {
        TValue obj;
        if (dictionary.TryGetValue(key, out obj))
        {
            return obj;
        }

        return dictionary[key] = factory(key);
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, k => factory());
    }

    /// <summary>
    /// Gets a value from the concurrent dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Concurrent dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, [NotNull] TKey key, Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, k => factory());
    }
}
