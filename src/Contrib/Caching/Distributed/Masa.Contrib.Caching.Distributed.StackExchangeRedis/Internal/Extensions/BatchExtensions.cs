// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis;

internal static class BatchExtensions
{
    public static Task<RedisValue[]> HashGetAsync(
        this IBatch batch,
        string key,
        params string[] hashFields)
        => batch.HashGetAsync(key, hashFields.ToRedisValueArray());
}
