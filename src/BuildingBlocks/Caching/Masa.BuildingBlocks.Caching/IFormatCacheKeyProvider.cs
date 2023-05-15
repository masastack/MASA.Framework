// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public interface IFormatCacheKeyProvider
{
    string FormatCacheKey<T>(string? instanceId, string key, CacheKeyType cacheKeyType, Func<string, string>? typeAliasFunc = null);
}
