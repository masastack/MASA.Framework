﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Distributed.Redis.Tests.Perfs;

/// <summary>
/// Only supports the use of Redis environment
/// </summary>
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 1000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 10000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100000)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class DistributedBenchmarks
{
    private IIdGenerator<long> _idGenerator;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddDistributedCache(distributedCacheOptions =>
        {
            distributedCacheOptions.UseStackExchangeRedisCache(new RedisConfigurationOptions()
            {
                Password = "",
                DefaultDatabase = 2,
                Servers = new List<RedisServerOptions>()
                {
                    new("127.0.0.1", 6379)
                }
            });
        });

        services.AddSnowflake(options =>
        {
            options.UseRedis();
            options.EnableMachineClock = true;
        });
        var serviceProvider = services.BuildServiceProvider();
        _idGenerator = serviceProvider.GetRequiredService<IIdGenerator<long>>();
        _idGenerator.NewId();
    }

    [Benchmark]
    public void Distributed()
    {
        _idGenerator.NewId();
    }
}
