// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

internal class SetOptions<T>
{
    public string? FormattedKey { get; set; }

    public T? Value { get; set; }

    public CacheEntryOptions? MemoryCacheEntryOptions { get; set; }
}
