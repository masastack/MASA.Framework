// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Caching.MultilevelCache.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.MultilevelCache;

internal static class MultilevelCacheConstant
{
    public const string DEFAULT_SECTION_NAME = "MultilevelCache";

    public const CacheKeyType DEFAULT_CACHE_KEY_TYPE = CacheKeyType.TypeName;
}
