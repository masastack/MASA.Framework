// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Utils;

public static class ListExtensions
{   
    public static List<TSource> MergeBy<TSource, TKey>(this IEnumerable<TSource> source, IEnumerable<TSource> target, Func<TSource, TKey> keySelector)
    {
        var removes = source.ExceptBy(target.Select(keySelector), keySelector);
        var adds = target.ExceptBy(source.Select(keySelector), keySelector);
        var merge = new List<TSource>().Union(source).ToList();
        merge.RemoveAll(data => removes.Contains(data));
        merge.AddRange(adds);
        return merge;
    }

    public static List<TSource> MergeBy<TSource, TKey>(this IEnumerable<TSource> source, IEnumerable<TSource> target, Func<TSource, TKey> keySelector, Func<TSource, TSource, TSource> replace) where TKey : notnull
    {
        var removes = source.ExceptBy(target.Select(keySelector), keySelector);
        var adds = target.ExceptBy(source.Select(keySelector), keySelector);
        var merge = new List<TSource>().Union(source).ToList();
        merge.RemoveAll(data => removes.Contains(data));
        var newMerge = merge.Select(item => replace(item, target.First(t => keySelector(item).Equals(keySelector(t))))).ToList();
        newMerge.AddRange(adds);

        return newMerge;
    }
}

