// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    internal class CacheItemModel<T>
    {
        public string Key { get; set; }

        public string MemoryCacheKey { get; set; }

        public bool IsExist { get; set; }

        public T? Value { get; set; }

        public CacheItemModel(string key, string memoryCacheKey, bool isExist, T? value)
        {
            Key = key;
            MemoryCacheKey = memoryCacheKey;
            IsExist = isExist;
            Value = value;
        }
    }
}
