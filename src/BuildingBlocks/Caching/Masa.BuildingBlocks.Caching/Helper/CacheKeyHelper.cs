// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public static class CacheKeyHelper
{
    public static string FormatCacheKey<T>(string key, CacheKeyType cacheKeyType)
    {
        switch (cacheKeyType)
        {
            case CacheKeyType.None:
                return key;
            case CacheKeyType.TypeName:
                return GetTypeName<T>() + key;
            default:
                throw new NotImplementedException();
        }
    }

    public static string GetTypeName<T>()
    {
        var type = typeof(T);
        if (type.IsGenericType)
        {
            var dictType = typeof(Dictionary<,>);
            if (type.GetGenericTypeDefinition() == dictType)
                return type.Name + "[" + type.GetGenericArguments()[1].Name + "]";

            return type.Name + "[" + type.GetGenericArguments()[0].Name + "]";
        }
        
        return typeof(T).Name;
    }
}
