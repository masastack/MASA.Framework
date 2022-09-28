// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Data.Prometheus.Test;

[TestClass]
public class MasaPrometheusClientTest
{
    private IMasaPrometheusClient _client = default!;
    private Mock<HttpMessageHandler> _mockHandler = new();
    private const string HOST = "http://localhost";

    [TestInitialize]
    public void Initialized()
    {
        IServiceCollection service = new ServiceCollection();
        service.AddLogging();
        var logger = service.BuildServiceProvider().GetRequiredService<ILogger<MasaPrometheusClient>>();

        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());
        _mockHandler = new();
        var client = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri(HOST)
        };
        _client = new MasaPrometheusClient(client, options, logger);
    }

    #region query test
    [TestMethod]
    [DataRow("up")]
    public async Task TestQueryOkAsync(string query)
    {
        SetTestData("{\"status\":\"success\",\"data\":{\"resultType\":\"vector\",\"result\":[{\"metric\":{\"__name__\":\"up\",\"instance\":\"host.docker.internal:9090\",\"job\":\"dapr\"},\"value\":[1659492293.921,\"1\"]}]}}");
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = query
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNotNull(result.Data.Result);
        Assert.IsTrue(result.Data.Result.Any());
    }

    [TestMethod]
    [DataRow("not_exists")]
    public async Task TestQueryEmptyAsync(string query)
    {
        SetTestData("{\"status\":\"success\",\"data\":{\"resultType\":\"vector\",\"result\":[]}}");
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = query
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNotNull(result.Data.Result);
        Assert.IsFalse(result.Data.Result.Any());
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("error data")]
    public async Task TestQueryErrorAsync(string query)
    {
        SetTestData("{\"status\":\"error\",\"errorType\":\"bad_data\",\"error\":\"invalid parameter \\\"query\\\": 1:7: parse error: unexpected identifier \\\"data\\\"\"}", HttpStatusCode.BadRequest);
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = query
        });

        Assert.IsNotNull(result);
        Assert.AreNotEqual(result.Status, ResultStatuses.Success);
    }
    #endregion

    [TestMethod]
    public async Task TestQueryVectorAsync()
    {
        SetTestData("{\"status\":\"success\",\"data\":{\"resultType\":\"vector\",\"result\":[{\"metric\":{\"__name__\":\"up\",\"instance\":\"host.docker.internal:9090\",\"job\":\"dapr\"},\"value\":[1659492293.921,\"1\"]}]}}");
        var result = await _client.QueryAsync(new QueryRequest
        {
            Query = "up"
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
        Assert.IsNotNull(result.Data.Result);
        Assert.IsTrue(result.Data.Result.Any());

        var data = (result.Data.Result as QueryResultInstantVectorResponse[])!;
        Assert.IsNotNull(data);
        var metrics = data[0].Metric;
        Assert.IsNotNull(metrics);
        Assert.IsNotNull(metrics.Keys);
        var values = data[0].Value;
        Assert.IsNotNull(values);
        Assert.IsTrue(values.Length > 0);
    }

    [TestMethod]
    public async Task TestQueryRangeAsync()
    {
        SetTestData("{\"status\":\"success\",\"data\":{\"resultType\":\"matrix\",\"result\":[{\"metric\":{\"__name__\":\"up\",\"instance\":\"172.31.96.1:8889\",\"job\":\"docker\"},\"values\":[[1659493800,\"0\"],[1659495600,\"0\"]]},{\"metric\":{\"__name__\":\"up\",\"instance\":\"host.docker.internal:9090\",\"job\":\"dapr\"},\"values\":[[1659493800,\"1\"],[1659495600,\"1\"]]}]}}");
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
        Assert.IsNotNull(result.Data.Result);
        Assert.IsTrue(result.Data.Result.Any());

        var data = result.Data.Result as QueryResultMatrixRangeResponse[];
        Assert.IsNotNull(data);
        Assert.IsNotNull(data[0].Metric);
        Assert.IsNotNull(data[0].Values);
    }

    [TestMethod]
    public async Task TestSeriesQueryAsync()
    {
        SetTestData("{\"status\":\"success\",\"data\":[{\"__name__\":\"up\",\"job\":\"prometheus\",\"instance\":\"localhost:9090\"},{\"__name__\":\"up\",\"job\":\"node\",\"instance\":\"localhost:9091\"},{\"__name__\":\"process_start_time_seconds\",\"job\":\"prometheus\",\"instance\":\"localhost:9090\"}]}");
        var result = await _client.SeriesQueryAsync(new MetaDataQueryRequest
        {
            Match = new string[] { "up" },
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z",
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    [DataRow(new string[] { "up", "up1" })]
    public async Task TestLabelsQueryAsync(IEnumerable<string> matches)
    {
        SetTestData("{\"status\":\"success\",\"data\":[\"up\"]}");
        var result = await _client.LabelsQueryAsync(new MetaDataQueryRequest
        {
            Match = new string[] { "up" },
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z",
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    public async Task TestLabelValuesQueryAsync()
    {
        SetTestData("{\"status\":\"success\",\"data\":[\"dapr\",\"docker\"]}");
        var result = await _client.LabelValuesQueryAsync(new LableValueQueryRequest
        {
            Lable = "job",
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z",
        });

        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
        Assert.IsNotNull(result.Data);
    }

    [TestMethod]
    [DataRow()]
    public async Task TestExemplarQueryAsync()
    {
        var str = "{\"status\":\"success\",\"data\":[{\"seriesLabels\":{\"__name__\":\"test_exemplar_metric_total\",\"instance\":\"localhost:8090\",\"job\":\"prometheus\",\"service\":\"bar\"},\"exemplars\":[{\"labels\":{\"traceID\":\"EpTxMJ40fUus7aGY\"},\"value\":\"6\",\"timestamp\":1600096945.479}]},{\"seriesLabels\":{\"__name__\":\"test_exemplar_metric_total\",\"instance\":\"localhost:8090\",\"job\":\"prometheus\",\"service\":\"foo\"},\"exemplars\":[{\"labels\":{\"traceID\":\"Olp9XHlq763ccsfa\"},\"value\":\"19\",\"timestamp\":1600096955.479},{\"labels\":{\"traceID\":\"hCtjygkIHwAN9vs4\"},\"value\":\"20\",\"timestamp\":1600096965.489}]}]}";
        SetTestData(str);
        var param = new QueryExemplarRequest
        {
            Query = "test_exemplar_metric_total",
            Start = "2022-06-17T02:00:00.000Z",
            End = "2022-06-17T02:30:00.000Z"
        };
        var result = await _client.ExemplarQueryAsync(param);
        Assert.IsNotNull(result);
        Assert.AreEqual(result.Status, ResultStatuses.Success);
    }

    private void SetTestData(string result, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(result)
           });
    }
}
