// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.Collections;

public static class CollectionExtensions
{
    public static bool IsNullOrEmpty<T>(this ICollection<T> source)
    {
        return source == null || !source.Any();
    }

    public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
    {
        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    public static void AddIfNotContains<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            if (source.Contains(item))
            {
                continue;
            }
            source.Add(item);
        }
    }

    public static int RemoveAll<T>(this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToList();
        foreach (var item in items)
        {
            source.Remove(item);
        }

        return items.Count;
    }

    public static void RemoveAll<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Remove(item);
        }
    }
}
