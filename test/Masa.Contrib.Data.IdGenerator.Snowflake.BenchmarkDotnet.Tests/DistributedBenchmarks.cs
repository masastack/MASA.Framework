﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.BenchmarkDotnet.Tests;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 1000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 10000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100000)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class DistributedBenchmarks
{
    private IIdGenerator _idGenerator;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddMasaRedisCache(opt =>
        {
            opt.Password = "";
            opt.DefaultDatabase = 2;
            opt.Servers = new List<RedisServerOptions>()
            {
                new("127.0.0.1", 6379)
            };
        });
        services.AddDistributedSnowflake(options => options.EnableMachineClock = true);
        var serviceProvider = services.BuildServiceProvider();
        _idGenerator = serviceProvider.GetRequiredService<IIdGenerator>();
        _idGenerator.Generate();
    }

    [Benchmark]
    public void DistributedGenerate()
    {
        _idGenerator.Generate();
    }
}
