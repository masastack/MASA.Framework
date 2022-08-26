// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Caching.Distributed.StackExchangeRedis.Options;

public class DistributedRedisCacheOptions
{
    public RedisConfigurationOptions? Options { get; set; }

    public JsonSerializerOptions? JsonSerializerOptions { get; set; }

    public SubscribeConfigurationOptions? SubscribeConfigurationOptions { get; set; }

    public CacheEntryOptions? CacheEntryOptions { get; set; }
}
