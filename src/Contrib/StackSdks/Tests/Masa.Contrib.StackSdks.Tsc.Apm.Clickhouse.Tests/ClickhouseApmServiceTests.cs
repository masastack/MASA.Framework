// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.StackSdks.Tsc.Clickhouse;

namespace Masa.Contrib.StackSdks.Tsc.Apm.Clickhouse.Tests;

[TestClass]
public class ClickhouseApmServiceTests
{
    private static IApmService _APMService;
    private static DateTime _start = DateTime.Parse("2024/01/03 22:00:00");

    [ClassInitialize]
    public static void Initialized(TestContext testContext)
    {
        var connection = new ClickHouseConnection(TestUtils.ConnectionString);
        Common.InitTable(false, connection);
        Common.InitTable(true, connection);
        var services = new ServiceCollection();
        services.AddLogging(builder => builder.AddConsole());
        services.AddMASAStackApmClickhouse(TestUtils.ConnectionString, "custom_log", "custom_trace");
        _APMService = services.BuildServiceProvider().GetRequiredService<IApmService>();
        Common.InitTableJsonData(false, AppDomain.CurrentDomain.BaseDirectory, connection);
        _start -= MasaStackClickhouseConnection.TimeZone.BaseUtcOffset;
    }

    [TestMethod]
    public async Task ServicePageAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            PageSize = 10,
            Page = 1
        };
        var result = await _APMService.ServicePageAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Total > 0);
        Assert.IsNotNull(result.Result);
    }

    [TestMethod]
    public async Task EndpointPageAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            PageSize = 10,
            Page = 1,
            Service = "tsc-service-iotdev"
        };
        var result = await _APMService.EndpointPageAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Total > 0);
        Assert.IsNotNull(result.Result);
    }

    [TestMethod]
    public async Task ChartDataAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            PageSize = 10,
            Page = 1,
            Service = "tsc-service-iotdev"
        };
        var result = await _APMService.ChartDataAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task EndpointLatencyDistributionAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            PageSize = 10,
            Page = 1,
            Service = "tsc-service-iotdev",
            Endpoint = "/api/trace/list"
        };
        var result = await _APMService.EndpointLatencyDistributionAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ErrorMessagePageAsync()
    {
        var query = new ApmEndpointRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            PageSize = 10,
            Page = 1,
            Service = "tsc-service-iotdev"
        };
        var result = await _APMService.ErrorMessagePageAsync(query);
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Total > 0);
    }

    [TestMethod]
    public async Task TraceLatencyDetailAsync()
    {
        var query = new ApmTraceLatencyRequestDto
        {
            Start = _start,
            End = _start.AddHours(1),
            ComparisonType = ComparisonTypes.DayBefore,
            StatusCodes = "401,402,503,500",
            Page = 1,
            Service = "tsc-service-iotdev",
            Endpoint = "/api/trace/list"
        };
        var result = await _APMService.TraceLatencyDetailAsync(query);
        Assert.IsNotNull(result);
        query.Env = "Development";
        query.LatMax = 1000000;
        result = await _APMService.TraceLatencyDetailAsync(query);
        Assert.IsNotNull(result);
    }
}
