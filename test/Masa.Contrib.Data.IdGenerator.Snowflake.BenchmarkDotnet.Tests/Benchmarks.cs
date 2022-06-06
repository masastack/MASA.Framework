// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.BenchmarkDotnet.Tests;

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 1000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 10000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100000)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Benchmarks
{
    private IIdGenerator<System.Snowflake, long> _idGenerator;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSnowflake();
        var serviceProvider = services.BuildServiceProvider();
        _idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        _idGenerator.Create();
    }

    [Benchmark]
    public void SnowflakeByMillisecond()
    {
        _idGenerator.Create();
    }
}

[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 1000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 10000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100000)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class SecondBenchmarks
{
    private IIdGenerator<System.Snowflake, long> _idGenerator;

    [GlobalSetup]
    public void GlobalSetup()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSnowflake(options => options.TimestampType = 2);
        var serviceProvider = services.BuildServiceProvider();
        _idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        _idGenerator.Create();
    }

    [Benchmark]
    public void SnowflakeBySecond()
    {
        _idGenerator.Create();
    }
}
