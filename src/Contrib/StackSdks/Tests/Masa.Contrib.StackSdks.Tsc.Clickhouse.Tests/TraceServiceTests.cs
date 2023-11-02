// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

[TestClass]
public class TraceServiceTests
{
    private ITraceService traceService;

    //[TestInitialize]
    public void Initialized()
    {
        var services = new ServiceCollection();
        services.AddMASAStackClickhouse(Consts.ConnectionString);
        traceService = services.BuildServiceProvider().GetRequiredService<ITraceService>();
    }

    [TestMethod]
    public async Task QueryListTest()
    {
        Initialized();
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Start = DateTime.Now.AddDays(-1),
            End = DateTime.Now
        };
        var result = await traceService.ListAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TraceIdTest()
    {
        Initialized();
        var result = await traceService.GetAsync("be85e016eee41870e2c65ace88979fbc");
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task AggTest()
    {
        Initialized();
        var request = new SimpleAggregateRequestDto
        {
            Name = "Resource.service.name",
            Type = AggregateTypes.Count,
            End = DateTime.Now,
            Start = DateTime.Now.AddDays(-5)
        };       
        var result = await traceService.AggregateAsync(request);
        //Assert.IsNotNull(result);
        //Assert.IsTrue(result is long);
        //var num1 = Convert.ToInt64(result);

        //request.Type = AggregateTypes.DistinctCount;
        //result = await traceService.AggregateAsync(request);
        //Assert.IsTrue(result is long);
        //var num2 = Convert.ToInt64(result);
        //Assert.IsTrue(num1 - num2 >= 0);

        //request.Type = AggregateTypes.GroupBy;
        //result= await traceService.AggregateAsync(request);
        //Assert.IsTrue(result is IEnumerable<string>);

        //request.Name = "Duration";
        //request.Type = AggregateTypes.Avg;
        //result=await traceService.AggregateAsync(request);
        //Assert.IsTrue(result is long);

        //request.Type = AggregateTypes.Sum;
        //result = await traceService.AggregateAsync(request);
        //Assert.IsTrue(result is long);

        request.Name = "Timestamp";
        request.Type = AggregateTypes.DateHistogram;
        request.Interval = "5m";
        result = await traceService.AggregateAsync(request);
        Assert.IsNotNull(result);
        Assert.IsTrue(result is IEnumerable<object>);
    }
}
