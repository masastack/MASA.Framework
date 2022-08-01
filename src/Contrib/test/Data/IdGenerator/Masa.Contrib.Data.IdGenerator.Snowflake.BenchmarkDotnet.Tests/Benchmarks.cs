// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.BenchmarkDotnet.Tests;

[MarkdownExporter, AsciiDocExporter, HtmlExporter]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 1000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 10000)]
[SimpleJob(RunStrategy.ColdStart, RuntimeMoniker.Net60, targetCount: 100000)]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class Benchmarks
{
    private IIdGenerator<System.Snowflake, long> _idGenerator;
    private IIdGenerator<System.Snowflake, long> _idGeneratorBySecond;
    private IIdGenerator<System.Snowflake, long> _idGeneratorByEnableMachineClock;
    private IIdGenerator<System.Snowflake, long> _idGeneratorBySecondAndEnableMachineClock;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _idGenerator = InitializeIdGenerator(services => services.AddSnowflake());
        _idGeneratorBySecond =
            InitializeIdGenerator(services => services.AddSnowflake(options => options.TimestampType = TimestampType.Seconds));
        _idGeneratorByEnableMachineClock =
            InitializeIdGenerator(services => services.AddSnowflake(options => options.EnableMachineClock = true));
        _idGeneratorBySecondAndEnableMachineClock =
            InitializeIdGenerator(services => services.AddSnowflake(options =>
            {
                options.EnableMachineClock = true;
                options.TimestampType = TimestampType.Seconds;
            }));
    }

    private IIdGenerator<System.Snowflake, long> InitializeIdGenerator(Action<IServiceCollection> action)
    {
        IServiceCollection services = new ServiceCollection();
        action.Invoke(services);
        var serviceProvider = services.BuildServiceProvider();
        var idGenerator = serviceProvider.GetRequiredService<IIdGenerator<System.Snowflake, long>>();
        idGenerator.NewId();
        return idGenerator;
    }

    [Benchmark(Baseline = true)]
    public void SnowflakeByMillisecond()
        => _idGenerator.NewId();

    [Benchmark]
    public void SnowflakeBySecond()
        => _idGeneratorBySecond.NewId();

    [Benchmark]
    public void SnowflakeByMillisecondAndEnableMachineClock()
        => _idGeneratorByEnableMachineClock.NewId();

    [Benchmark]
    public void SnowflakeBySecondAndEnableMachineClock()
        => _idGeneratorBySecondAndEnableMachineClock.NewId();
}
