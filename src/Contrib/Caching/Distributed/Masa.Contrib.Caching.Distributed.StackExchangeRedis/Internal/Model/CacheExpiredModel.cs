// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal class CacheExpiredModel
{
    public long AbsoluteExpirationTicks { get; set; }

    public long SlidingExpirationTicks { get; set; }

    public long Expired { get; set; }

    public CacheExpiredModel(long absoluteExpirationTicks, long slidingExpirationTicks, long expired)
    {
        AbsoluteExpirationTicks = absoluteExpirationTicks;
        SlidingExpirationTicks = slidingExpirationTicks;
        Expired = expired;
    }
}
