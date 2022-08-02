// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Test;

[TestClass]
public class MasaPrometheusClientTests
{
    private IMasaPrometheusClient _client;

    public MasaPrometheusClientTests()
    {
        IServiceCollection service = new ServiceCollection();
        service.AddPrometheusClient("http://localhost:9090");
        _client = service.BuildServiceProvider().GetService<IMasaPrometheusClient>() ?? default!;
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("up")]
    [DataRow("not_exists")]
    [DataRow("error data")]
    public async Task TestQueryAsync(string query)
    {
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = query
        });

        Assert.IsNotNull(result);
        if (string.IsNullOrEmpty(query) || query.Contains(' '))
        {
            Assert.AreEqual(result.Status, ResultStatuses.Error);
        }
        else
        {
            if (query == "not_exists")
            {
                Assert.IsFalse(result.Data?.Result?.Any());
            }
            else
            {
                Assert.IsTrue(result.Data?.Result?.Any());
            }
        }
    }

    [TestMethod]
    public async Task TestQueryVectorAsync()
    {
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = "up"
        });

        if (result != null && result.Data != null && result.Data.Result != null)
        {
            var data = result.Data.Result as QueryResultInstantVectorResponse[];

            Assert.IsNotNull(data);
            Assert.IsNotNull(data[0].Metric);
            Assert.IsNotNull(data[0].Value);
            Assert.IsNotNull(data[0].Metric.Keys);
            Assert.AreEqual(2, data[0].Value.Length);
        }
    }

    [TestMethod]
    public async Task TestQueryRangeAsync()
    {
        var result = await _client.QueryRangeAsync(new QueryRangeRequest
        {
            Query = "up",
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z",
            Step = "300s",
        });
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
        if (result.Data.ResultType == ResultTypes.Matrix)
        {
            var data = result.Data.Result as QueryResultMatrixRangeResponse[];
            Assert.IsNotNull(data);
            Assert.IsNotNull(data[0].Metric);
            Assert.IsNotNull(data[0].Values);
        }
    }

    [TestMethod]
    public async Task TestSeriesQueryAsync()
    {
        var result = await _client.SeriesQueryAsync(new MetaDataQueryRequest
        {
            Match = new string[] { "up" },
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z"
        });
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow(new string[] { "up" })]
    [DataRow(new string[] { "not_exists" })]
    [DataRow(new string[] { "error data" })]
    public async Task TestLabelsQueryAsync(IEnumerable<string> matches)
    {
        if (matches != null && matches.Any(s => s.Contains(' ')))
        {
            var result = await _client.LabelsQueryAsync(new MetaDataQueryRequest { Match = matches });
            Assert.AreEqual(result.Status, ResultStatuses.Error);
            Assert.IsNotNull(result.Error);
        }
        else
        {
            var result = await _client.LabelsQueryAsync(default!);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, ResultStatuses.Success);
            if (matches == null || matches.Any(s => s == "up"))
            {
                Assert.IsTrue(result.Data?.Any());
            }
            else
            {
                Assert.IsFalse(result.Data?.Any());
            }
        }
    }

    [TestMethod]
    public async Task TestLabelValuesQueryAsync()
    {
        var result = await _client.LabelValuesQueryAsync(new LableValueQueryRequest());
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
    }

    [TestMethod]
    [DataRow()]
    public async Task TestExemplarQueryAsync()
    {
        var param = new QueryExemplarRequest
        {
            Query = "up",
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z"
        };
        var result = await _client.ExemplarQueryAsync(param);
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
    }
}
