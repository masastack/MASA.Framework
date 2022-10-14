// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public static class CacheKeyHelper
{
    public static string FormatCacheKey<T>(string key, CacheKeyType cacheKeyType, Func<string, string>? typeAliasFunc = null)
    {
        switch (cacheKeyType)
        {
            case CacheKeyType.None:
                return key;
            case CacheKeyType.TypeName:
                return $"{GetTypeName<T>()}.{key}";
            case CacheKeyType.TypeAlias:
                if (typeAliasFunc == null)
                    throw new NotImplementedException();

                var typeName = GetTypeName<T>();
                return $"{typeAliasFunc.Invoke(typeName)}:{key}";
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
