// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Tests.Service;

[TestClass]
public class MetricServiceTests
{
    private ITscClient _client;

    [TestInitialize]
    public void Initialize()
    {
        IServiceCollection service = new ServiceCollection();
        service.AddTscClient("https://localhost:6324/");
        _client = service.BuildServiceProvider().GetRequiredService<ITscClient>();
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow(new string[] { "up", "prometheus_http_requests_total", "prometheus_http_request_duration_seconds_count" })]
    [DataRow(new string[] { "not_exists", "up" })]
    [DataRow(new string[] { "not_exists" })]
    public async Task GetNamesAsyncTest(IEnumerable<string> match)
    {
        var result = await _client.MetricService.GetNamesAsync(match);
        if (match == null)
        {
            Assert.IsNotNull(result);
        }
        else if (match.Count() > 0)
        {
            Assert.IsNotNull(result);
        }
        else
        {
            Assert.IsNull(result);
        }
    }

    [TestMethod]
    [DataRow("up", "2022-07-01T09:00:00.000Z", "2022-07-05T22:00:00.000Z")]
    public async Task GetLabelValuesAsyncTest(string match, string start, string end)
    {
        DateTime startDateTime = DateTime.Parse(start);
        DateTime endDateTime = DateTime.Parse(end);
        var result = await _client.MetricService.GetLabelValuesAsync(new LableValuesRequest
        {
            Match = match,
            Start = startDateTime,
            End = endDateTime
        });

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    [TestMethod]
    [DataRow("up", null, "2022-07-01T09:00:00.000Z", "2022-07-05T22:00:00.000Z")]
    public async Task GetValuesAsyncTest(string match, IEnumerable<string> labels, string start, string end)
    {
        DateTime startDateTime = DateTime.Parse(start);
        DateTime endDateTime = DateTime.Parse(end);
        var result = await _client.MetricService.GetValuesAsync(new ValuesRequest
        {
            Match = match,
            Lables = labels,
            End = endDateTime,
            Start = startDateTime
        });
        Assert.IsNotNull(result);
    }
}
