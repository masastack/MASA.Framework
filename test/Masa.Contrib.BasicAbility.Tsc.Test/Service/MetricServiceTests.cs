// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Tests;

[TestClass]
public class MetricServiceTests
{
    [TestMethod]
    public async Task GetMetricNamesAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();
        IEnumerable<string> data = new string[] { "up", "http_request_total" };
        callerProvider.Setup(provider => provider.GetAsync<IEnumerable<string>>(MetricService.NAMES_URI, default)).ReturnsAsync(data).Verifiable();
        IEnumerable<string> query = new string[] { "up" };
        callerProvider.Setup(provider => provider.GetAsync<IEnumerable<string>>(MetricService.NAMES_URI, query, default)).ReturnsAsync(query).Verifiable();
        IEnumerable<string> queryNotExists = new string[] { "up1" };
        callerProvider.Setup(provider => provider.GetAsync<IEnumerable<string>>(MetricService.NAMES_URI, queryNotExists, default)).ReturnsAsync(default(IEnumerable<string>)).Verifiable();
        var client = new TscClient(callerProvider.Object);

        var result = await client.MetricService.GetMetricNamesAsync(default);
        Assert.IsNotNull(result);

        result = await client.MetricService.GetMetricNamesAsync(query);
        Assert.IsNotNull(result);

        result = await client.MetricService.GetMetricNamesAsync(queryNotExists);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetLabelAndValuesAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();

        var time = DateTime.Now;
        var query = new MetricLableValuesRequest
        {
            Match = "up",
            Start = time.AddMinutes(-15),
            End = time
        };
        var data = new Dictionary<string, List<string>>
        {
            {"job",new List<string>{"prometheus"} },
            { "container",new List<string>{ "config-reloader", "coredns" } }
        };
        callerProvider.Setup(provider => provider.GetAsync<Dictionary<string, List<string>>>(MetricService.LABELVALUES_URI, query, default)).ReturnsAsync(data).Verifiable();
        var client = new TscClient(callerProvider.Object);

        var result = await client.MetricService.GetLabelAndValuesAsync(query);
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetMetricValuesAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();

        var time = DateTime.Now;
        var query = new MetricRangeValueRequest
        {
            Match = "up",
            Start = time.AddMinutes(-15),
            End = time
        };
        var data = "100";
        callerProvider.Setup(provider => provider.GetAsync<string>(MetricService.RANGEVALUES_URL, query, default)).ReturnsAsync(data).Verifiable();
        var client = new TscClient(callerProvider.Object);

        var result = await client.MetricService.GetMetricValuesAsync(query);
        Assert.IsNull(result);
    }
}
