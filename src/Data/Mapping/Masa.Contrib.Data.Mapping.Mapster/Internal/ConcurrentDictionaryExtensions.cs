// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Internal;

internal static class ConcurrentDictionaryExtensions
{
    public static TValue GetOrAdd<TKey, TValue>(
        this ConcurrentDictionary<TKey, Lazy<TValue>> concurrentDictionary,
        TKey key,
        Func<TKey, TValue> valueFactory) where TKey : notnull
    {
        return concurrentDictionary.GetOrAdd(key, key => new Lazy<TValue>(() => valueFactory(key))
        ).Value;
    }
}
