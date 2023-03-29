// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticsearch.Tests;

[TestClass]
public class TraceServiceTests
{
    private ITraceService _traceService;

    [ClassInitialize]
    public static void InitializeTrace(TestContext testContext)
    {
        var mapping = StaticConfig.GetJson("Mapping:trace");
        var httpJson = StaticConfig.GetJson("Init:traceHttp");
        var databaseJson = StaticConfig.GetJson("Init:traceDatabase");
        var exceptionJson = StaticConfig.GetJson("Init:traceException");

        var httpClient = new HttpClient() { BaseAddress = new Uri(StaticConfig.HOST) };
        httpClient.Send(StaticConfig.CreateMessage(StaticConfig.TRACE_INDEX_NAME, HttpMethod.Delete));
        httpClient.Send(StaticConfig.CreateMessage(StaticConfig.TRACE_INDEX_NAME, HttpMethod.Put, mapping));
        httpClient.Send(StaticConfig.CreateMessage($"{StaticConfig.TRACE_INDEX_NAME}/_doc", HttpMethod.Post, httpJson));
        httpClient.Send(StaticConfig.CreateMessage($"{StaticConfig.TRACE_INDEX_NAME}/_doc", HttpMethod.Post, databaseJson));
        httpClient.Send(StaticConfig.CreateMessage($"{StaticConfig.TRACE_INDEX_NAME}/_doc", HttpMethod.Post, exceptionJson));
        Task.Delay(1000).Wait();
    }

    [TestInitialize]
    public void Initialize()
    {
        ServiceCollection services = new();
        services.AddElasticClientTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST }).
            UseConnectionSettings(setting => setting.EnableApiVersioningHeader(false));
        },
            callerBuilder =>
        {
            callerBuilder.BaseAddress = StaticConfig.HOST;
        },
        StaticConfig.TRACE_INDEX_NAME);
        var serviceProvider = services.BuildServiceProvider();
        _traceService = serviceProvider.GetRequiredService<ITraceService>();
    }

    [TestMethod]
    public async Task QueryTest()
    {
        Assert.IsNotNull(ElasticConstant.Trace.Mappings.Value);

        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Sort =
                new FieldOrderDto
                {
                    Name = ElasticConstant.ServiceName
                }
        };

        var result = await _traceService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());
    }

    [TestMethod]
    public async Task GetTest()
    {
        var result = await _traceService.GetAsync("277df6d0204f09fa63ff4ab896673455");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        bool isHttp = result.First().TryParseHttp(out var httpDto);
        Assert.IsTrue(isHttp);
        Assert.IsNotNull(httpDto);
        Assert.IsNotNull(httpDto.Method);

        result = await _traceService.GetAsync("notexits");
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Any());

        //exception
        result = await _traceService.GetAsync("1feb13f2c4b882a97db0ef7d43231d6d");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        bool isException = result.First().TryParseException(out var exceptionDto);
        Assert.IsTrue(isException);
        Assert.IsNotNull(exceptionDto);
        Assert.IsNotNull(exceptionDto.Message);

        result = await _traceService.GetAsync("894c8d019af8f422e660581da8d1f165");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        bool isDatabase = result.First().TryParseDatabase(out var databaseDto);
        Assert.IsTrue(isDatabase);
        Assert.IsNotNull(databaseDto);
        Assert.AreEqual("mssql", databaseDto.System);
    }

    [TestMethod]
    public async Task AggTest()
    {
        var query = new SimpleAggregateRequestDto
        {
            Service = "masa-tsc-web-admin",
            Name = ElasticConstant.ServiceName,
            Type = AggregateTypes.GroupBy,
            MaxCount = 10
        };
        var result = await _traceService.AggregateAsync(query);
        Assert.IsNotNull(result);

        var values = (IEnumerable<string>)result;
        Assert.IsNotNull(values);
        Assert.IsTrue(values.All(s => !string.IsNullOrEmpty(s)));
    }

    [TestMethod]
    public async Task GetScrollAsyncTest()
    {
        var query = new ElasticsearchScrollRequestDto
        {
            Scroll = "1m",
            PageSize = 1
        };
        var firstResult = await _traceService.ScrollAsync(query);

        Assert.IsNotNull(firstResult);
        Assert.IsNotNull(firstResult.Result);

        var elasticsearchScrollResult = (ElasticsearchScrollResponseDto<TraceResponseDto>)firstResult;
        Assert.IsNotNull(elasticsearchScrollResult.ScrollId);
        query.ScrollId = elasticsearchScrollResult.ScrollId;

        var secondResult = await _traceService.ScrollAsync(query);

        Assert.IsNotNull(secondResult);
        Assert.IsTrue(secondResult.Result.Any());
    }
}
