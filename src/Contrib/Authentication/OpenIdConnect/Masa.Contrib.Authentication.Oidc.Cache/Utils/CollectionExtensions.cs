// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.Cache.Utils;

public static class CollectionExtensions
{
    public static void Set<T>(this ICollection<T> collection, T data, Func<T, object> keySelector)
    {
        collection.Remove(data, keySelector);
        collection.Add(data);
    }

    public static void SetRange<T>(this ICollection<T> collection, IEnumerable<T> datas, Func<T, object> keySelector)
    {
        collection.RemoveRange(datas, keySelector);
        foreach (var data in datas)
        {
            collection.Add(data);
        }
    }

    public static void Remove<T>(this ICollection<T> collection, T data, Func<T, object> keySelector)
    {
        var oldData = collection.FirstOrDefault(item => keySelector(item).Equals(keySelector(data)));
        if (oldData is not null) collection.Remove(oldData);
    }

    public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> datas, Func<T, object> keySelector)
    {
        var oldDatas = collection.Where(item => datas.Any(data => keySelector(data).Equals(keySelector(item)))).ToList();
        if (oldDatas.Count() > 0)
        {
            foreach (var oldData in oldDatas)
            {
                collection.Remove(oldData);
            }
        }
    }

    public static void Remove<T>(this ICollection<T> collection, Func<T, bool> condition)
    {
        var datas = collection.Where(condition).ToList();
        if(datas.Count() > 0)
        {
            foreach (var data in datas)
            {
                collection.Remove(data);
            }
        }
    }
}
