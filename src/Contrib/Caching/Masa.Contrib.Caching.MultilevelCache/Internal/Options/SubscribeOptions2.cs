// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.MultilevelCache;

/// <summary>
/// todo: 需要更改类名
/// </summary>
internal class SubscribeOptions2<T>
{
    public CacheEntryOptions? MemoryCacheEntryOptions { get; set; }

    public Action<T?>? ValueChanged { get; set; }
}
