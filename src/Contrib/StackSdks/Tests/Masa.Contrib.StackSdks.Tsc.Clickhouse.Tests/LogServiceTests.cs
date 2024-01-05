// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Clickhouse.Tests;

[TestClass]
public class LogServiceTests
{
    private static ILogService logService;
    private readonly DateTime startTime = DateTime.Parse("2023-11-02 09:00:00");

    [ClassInitialize]
    public static void Initialized(TestContext testContext)
    {
        var connection = new ClickHouseConnection(Consts.ConnectionString);
        Common.InitTable(true, connection);
        Common.InitTable(false, connection);
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddMASAStackClickhouse(Consts.ConnectionString, "custom_log", "custom_trace");
        Common.InitTableData(true, AppDomain.CurrentDomain.BaseDirectory, connection);
        logService = services.BuildServiceProvider().GetRequiredService<ILogService>();
    }

    [TestMethod]
    public async Task QueryListTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Start = startTime,
            End = startTime.AddHours(1),
            Keyword = "Kafka",
            Conditions = new List<FieldConditionDto> {
                                                new FieldConditionDto{
                                                    Name="Resource.service.name",
                                                    Type= ConditionTypes.Equal,
                                                    Value="service"
                                                },
                                                new FieldConditionDto{
                                                    Name="Resource.service.namespace",
                                                    Type=ConditionTypes.NotEqual,
                                                    Value="Test"
                                                },
                                                new FieldConditionDto{
                                                        Name="Resource.service.name",
                                                        Type=ConditionTypes.In,
                                                        Value=new List<string>{ "service" }
                                                },
                                                new FieldConditionDto{
                                                    Name="Resource.service.name",
                                                    Type= ConditionTypes.NotIn,
                                                    Value=new List<string>{"a","b" }
                                                }
            },
        };
        var result = await logService.ListAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task MappingTest()
    {
        var mapping = await logService.GetMappingAsync();
        Assert.IsNotNull(mapping);
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
        var result = await logService.AggregateAsync(request);
        Assert.IsNotNull(result);
        var num1 = Convert.ToInt64(result);

        request.Name = "Timestamp";
        request.Type = AggregateTypes.DateHistogram;
        request.Interval = "5m";
        result = await logService.AggregateAsync(request);
        Assert.IsNotNull(result);
        Assert.IsTrue(result is IEnumerable<object>);
    }
}
