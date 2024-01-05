// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

[TestClass]
public class TraceServiceTests
{
    private static ITraceService traceService;
    private readonly DateTime startTime = DateTime.Parse("2023-11-02 09:00:00");

    [ClassInitialize]
    public static void Initialized(TestContext testContext)
    {
        var services = new ServiceCollection();
        var connection = new ClickHouseConnection(Consts.ConnectionString);
        services.AddLogging(builder => builder.AddConsole());
        services.AddMASAStackClickhouse(Consts.ConnectionString, "custom_log", "custom_trace");
        Common.InitTableData(false, AppDomain.CurrentDomain.BaseDirectory, connection);
        traceService = services.BuildServiceProvider().GetRequiredService<ITraceService>();
    }

    [TestMethod]
    public async Task QueryListTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Start = startTime,
            End = startTime.AddHours(1)
        };
        var result = await traceService.ListAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TraceIdTest()
    {
        var result = await traceService.GetAsync("3a749e0df4bde3713ea47ed0b8efe83f");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AggTest()
    {
        var request = new SimpleAggregateRequestDto
        {
            Name = "Resource.service.name",
            Type = AggregateTypes.Count,
            Start = startTime,
            End = startTime.AddHours(1),
        };
        var result = await traceService.AggregateAsync(request);
        Assert.IsNotNull(result);
        var num1 = Convert.ToInt64(result);

        request.Type = AggregateTypes.DistinctCount;
        result = await traceService.AggregateAsync(request);
        var num2 = Convert.ToInt64(result);
        Assert.IsTrue(num1 - num2 >= 0);

        request.Type = AggregateTypes.GroupBy;
        result = await traceService.AggregateAsync(request);
        Assert.IsTrue(result is IEnumerable<string>);

        request.Name = "Duration";
        request.Type = AggregateTypes.Avg;
        result = await traceService.AggregateAsync(request);

        request.Type = AggregateTypes.Sum;
        result = await traceService.AggregateAsync(request);

        request.Name = "Timestamp";
        request.Type = AggregateTypes.DateHistogram;
        request.Interval = "5m";
        result = await traceService.AggregateAsync(request);
        Assert.IsNotNull(result);
        Assert.IsTrue(result is IEnumerable<object>);
    }
}
