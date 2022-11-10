// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Tests.Service;

[TestClass]
public class MetricServiceTests
{
    [TestMethod]
    [DataRow(null)]
    [DataRow(new string[] { "up", "prometheus_http_requests_total", "prometheus_http_request_duration_seconds_count" })]
    [DataRow(new string[] { "not_exists_test" })]
    public async Task GetNamesAsyncTest(IEnumerable<string> match)
    {
        var data = new string[] { "up", "prometheus_http_requests_total", "prometheus_http_request_duration_seconds_count" };
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.GetAsync<IEnumerable<string>?>(MetricService.NAMES_URI, It.Is<Dictionary<string, string>>(dic => dic == null || dic.ContainsKey("match")), default))
            .ReturnsAsync((string? url, Dictionary<string, string> param, CancellationToken token) =>
        {
            if (param == null || !param.ContainsKey("match") || param["match"] is null || !param["match"].Contains("not_exists_test"))
                return data;
            return default;
        }).Verifiable();
        var client = new TscClient(caller.Object);
        var result = await client.MetricService.GetNamesAsync(match);

        if (match != null && match.Any(s => s == "not_exists_test"))
        {
            Assert.IsNull(result);
        }
        else
        {
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Any());
        }
    }

    [TestMethod]
    [DataRow("up", "2022-07-01T09:00:00.000Z", "2022-07-05T22:00:00.000Z")]
    public async Task GetLabelValuesAsyncTest(string match, string start, string end)
    {
        var caller = new Mock<ICaller>();


        caller.Setup(provider => provider.SendAsync<Dictionary<string, Dictionary<string, List<string>>>>(It.IsNotNull<HttpRequestMessage>(), default))
            .ReturnsAsync(new Dictionary<string, Dictionary<string, List<string>>> {
                    {"up",new Dictionary<string, List<string>>{
                    {"name",new List<string>{"name1","name2"} }
                } }
            });
        var client = new TscClient(caller.Object);

        DateTime startDateTime = DateTime.Parse(start);
        DateTime endDateTime = DateTime.Parse(end);
        var query = new LableValuesRequest
        {
            Match = match,
            Start = startDateTime,
            End = endDateTime
        };
        var result = await client.MetricService.GetLabelValuesAsync(query);
        Assert.AreEqual(match, query.Matches.First());
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
    }

    [TestMethod]
    [DataRow("up", null, "2022-07-01T09:00:00.000Z", "2022-07-05T22:00:00.000Z")]
    public async Task GetValuesAsyncTest(string match, IEnumerable<string> labels, string start, string end)
    {
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.SendAsync<string>(It.IsNotNull<HttpRequestMessage>(), default)).ReturnsAsync("1.0");
        var client = new TscClient(caller.Object);

        DateTime startDateTime = DateTime.Parse(start);
        DateTime endDateTime = DateTime.Parse(end);
        var result = await client.MetricService.GetValuesAsync(new ValuesRequest
        {
            Match = match,
            Lables = labels,
            End = endDateTime,
            Start = startDateTime
        });
        Assert.IsNotNull(result);
    }
}
