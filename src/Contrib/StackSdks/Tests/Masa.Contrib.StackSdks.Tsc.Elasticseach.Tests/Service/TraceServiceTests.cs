// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Service;

[TestClass]
public class TraceServiceTests
{
    private ITraceService _traceService;

    [ClassInitialize]
    public static void InitializeTrace(TestContext testContext)
    {
        string strHttpJson = "{\"@timestamp\":\"2022-11-15T12:12:10.938123800Z\",\"Attributes.host.name\":\"SSKJ016\",\"Attributes.http.client_ip\":\"::1\",\"Attributes.http.flavor\":\"HTTP/2\",\"Attributes.http.host\":\"localhost:18012\",\"Attributes.http.method\":\"GET\",\"Attributes.http.response_content_length\":0,\"Attributes.http.scheme\":\"https\",\"Attributes.http.status_code\":302,\"Attributes.http.target\":\"/\",\"Attributes.http.url\":\"https://localhost:18012/\",\"Attributes.http.user_agent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36\",\"EndTimestamp\":\"2022-11-15T12:12:11.323348000Z\",\"Kind\":\"SPAN_KIND_SERVER\",\"Link\":\"[]\",\"Name\":\"/\",\"Resource.service.instance.id\":\"9eaf1452-8b12-4368-928c-48f871c250fe\",\"Resource.service.name\":\"masa-tsc-web-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SpanId\":\"1c495129b86de343\",\"TraceId\":\"277df6d0204f09fa63ff4ab896673455\",\"TraceStatus\":0}";
        string strExceptionJson = "{\"@timestamp\":\"2022-11-15T12:11:57.477146800Z\",\"Attributes.exception.message\":\"SearchAsync execute error elastic query error: status:404,message:ServerError: 404Type: index_not_found_exception Reason: \\\"no such index [masa-stack-trace-0.61]\\\"\",\"Attributes.exception.stacktrace\":\"System.UserFriendlyException: SearchAsync execute error elastic query error: status:404,message:ServerError: 404Type: index_not_found_exception Reason: \\\"no such index [masa-stack-trace-0.61]\\\"\\r\\n   at Nest.IElasticClientExtenstion.SearchAsync[TResult,TQuery](IElasticClient client, String indexName, TQuery query, Func`3 condition, Action`2 result, Func`3 aggregate, Func`1 page, Func`3 sort, Func`1 includeFields, Func`1 excludeFields) in D:\\\\workplaces\\\\MASA.Framework\\\\src\\\\Contrib\\\\StackSdks\\\\Masa.Contrib.StackSdks.Tsc.Elasticseach\\\\Extenistions\\\\IElasticClientExtenstion.cs:line 65\\r\\n   at Nest.IElasticClientExtenstion.AggregateTraceAsync(IElasticClient client, SimpleAggregateRequestDto query) in D:\\\\workplaces\\\\MASA.Framework\\\\src\\\\Contrib\\\\StackSdks\\\\Masa.Contrib.StackSdks.Tsc.Elasticseach\\\\Extenistions\\\\IElasticClientExtenstion.cs:line 227\\r\\n   at Masa.Contrib.StackSdks.Tsc.Log.Elasticseach.TraceService.AggregateAsync(SimpleAggregateRequestDto query) in D:\\\\workplaces\\\\MASA.Framework\\\\src\\\\Contrib\\\\StackSdks\\\\Masa.Contrib.StackSdks.Tsc.Elasticseach\\\\TraceService.cs:line 17\\r\\n   at Masa.Tsc.Service.Admin.Application.Traces.QueryHandler.GetAttrValuesAsync(TraceAttrValuesQuery query) in D:\\\\workplaces\\\\MASA.TSC\\\\src\\\\Services\\\\Masa.Tsc.Service.Admin\\\\Application\\\\Traces\\\\QueryHandler.cs:line 60\\r\\n   at Masa.Contrib.Dispatcher.Events.EventHandlerAttribute.ExecuteAction[TEvent](IServiceProvider serviceProvider, TEvent event)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Dispatch.DispatcherBase.<>c__DisplayClass7_0`1.<<ExecuteEventHandlerAsync>b__0>d.MoveNext()\\r\\n--- End of stack trace from previous location ---\\r\\n   at Masa.Contrib.Dispatcher.Events.Strategies.ExecutionStrategy.ExecuteAsync[TEvent](StrategyOptions strategyOptions, TEvent event, Func`2 func, Func`4 cancel)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.DispatcherExtensions.ThrowException(Exception exception)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Dispatch.DispatcherBase.<>c__DisplayClass7_1`1.<<ExecuteEventHandlerAsync>b__1>d.MoveNext()\\r\\n--- End of stack trace from previous location ---\\r\\n   at Masa.Contrib.Dispatcher.Events.Strategies.ExecutionStrategy.ExecuteAsync[TEvent](StrategyOptions strategyOptions, TEvent event, Func`2 func, Func`4 cancel)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Dispatch.DispatcherBase.ExecuteEventHandlerAsync[TEvent](IServiceProvider serviceProvider, List`1 dispatchRelations, TEvent event)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Dispatch.DispatcherBase.PublishEventAsync[TEvent](IServiceProvider serviceProvider, TEvent event)\\r\\n   at Masa.Contrib.Dispatcher.Events.EventBus.<>c__DisplayClass7_0`1.<<PublishAsync>b__1>d.MoveNext()\\r\\n--- End of stack trace from previous location ---\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Middleware.TransactionMiddleware`1.HandleAsync(TEvent event, EventHandlerDelegate next)\\r\\n   at Masa.Contrib.Dispatcher.Events.Internal.Middleware.TransactionMiddleware`1.HandleAsync(TEvent event, EventHandlerDelegate next)\\r\\n   at Masa.Tsc.Service.Infrastructure.Middleware.LogMiddleware`1.HandleAsync(TEvent action, EventHandlerDelegate next) in D:\\\\workplaces\\\\MASA.TSC\\\\src\\\\Services\\\\Masa.Tsc.Service.Admin\\\\Infrastructure\\\\Middleware\\\\LogMiddleware.cs:line 24\\r\\n   at Masa.Contrib.Dispatcher.Events.EventBus.PublishAsync[TEvent](TEvent event)\\r\\n   at Masa.Tsc.Service.Admin.Services.TraceService.GetAttrValuesAsync(IEventBus eventBus, TraceAggregateRequestDto model) in D:\\\\workplaces\\\\MASA.TSC\\\\src\\\\Services\\\\Masa.Tsc.Service.Admin\\\\Services\\\\TraceService.cs:line 33\\r\\n   at Microsoft.AspNetCore.Http.RequestDelegateFactory.<ExecuteTask>g__ExecuteAwaited|58_0[T](Task`1 task, HttpContext httpContext)\\r\\n   at Microsoft.AspNetCore.Http.RequestDelegateFactory.<>c__DisplayClass46_3.<<HandleRequestBodyAndCompileRequestDelegate>b__2>d.MoveNext()\\r\\n--- End of stack trace from previous location ---\\r\\n   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|6_0(Endpoint endpoint, Task requestTask, ILogger logger)\\r\\n   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)\\r\\n   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)\\r\\n   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)\\r\\n   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)\\r\\n   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)\",\"Attributes.host.name\":\"SSKJ016\",\"Attributes.http.client_ip\":\"::1\",\"Attributes.http.flavor\":\"HTTP/1.1\",\"Attributes.http.host\":\"localhost:18010\",\"Attributes.http.method\":\"GET\",\"Attributes.http.request_content_body\":\"{\\r\\n  \\\"page\\\": 0,\\r\\n  \\\"size\\\": 0,\\r\\n  \\\"keyword\\\": \\\"\\\",\\r\\n  \\\"start\\\": \\\"2022-11-15T01:46:06.375Z\\\",\\r\\n  \\\"end\\\": \\\"2022-11-15T11:46:06.375Z\\\",\\r\\n  \\\"rawQuery\\\": \\\"\\\",\\r\\n    \\r\\n  \\\"type\\\": 6,\\r\\n  \\\"maxCount\\\": 10,\\r\\n  \\\"interval\\\": \\\"\\\",\\r\\n  \\\"traceId\\\": \\\"\\\",\\r\\n  \\\"service\\\": \\\"masa-tsc-service-admin\\\",\\r\\n  \\\"instance\\\": \\\"df7f15cc-0e17-48b0-93d1-f0bc7e886be3\\\",\\r\\n  \\\"endpoint\\\": \\\"\\\"\\r\\n}\",\"Attributes.http.request_content_length\":336,\"Attributes.http.request_content_type\":\"application/json\",\"Attributes.http.response_content_type\":\"text/plain; charset=utf-8\",\"Attributes.http.scheme\":\"https\",\"Attributes.http.status_code\":500,\"Attributes.http.target\":\"/api/trace/attr-values\",\"Attributes.http.url\":\"https://localhost:18010/api/trace/attr-values\",\"Attributes.http.user_agent\":\"PostmanRuntime/7.29.2\",\"EndTimestamp\":\"2022-11-15T12:12:02.989775300Z\",\"Kind\":\"SPAN_KIND_SERVER\",\"Link\":\"[]\",\"Name\":\"/api/trace/attr-values\",\"Resource.service.instance.id\":\"b17469f1-7243-4d4c-a921-76b84f358d3e\",\"Resource.service.name\":\"masa-tsc-service-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SpanId\":\"7fe4e67c31d2a1ce\",\"TraceId\":\"1feb13f2c4b882a97db0ef7d43231d6d\",\"TraceStatus\":2}";
        string strDbJson = "{\"@timestamp\":\"2022-11-15T12:14:19.353985100Z\",\"Attributes.db.name\":\"tsc_dev\",\"Attributes.db.statement\":\"SELECT TOP(@__p_2) [i].[Id], [i].[Content], [i].[CreationTime], [i].[EventId], [i].[EventTypeName], [i].[ModificationTime], [i].[RowVersion], [i].[State], [i].[TimesSent], [i].[TransactionId]\\r\\nFROM [tsc].[IntegrationEventLog] AS [i]\\r\\nWHERE ([i].[State] IN (3, 1) AND ([i].[TimesSent] <= @__maxRetryTimes_0)) AND ([i].[ModificationTime] < @__time_1)\\r\\nORDER BY [i].[CreationTime]\",\"Attributes.db.statement_type\":\"Text\",\"Attributes.db.system\":\"mssql\",\"Attributes.peer.service\":\"10.175.171.201,32679\",\"EndTimestamp\":\"2022-11-15T12:14:21.210334300Z\",\"Kind\":\"SPAN_KIND_CLIENT\",\"Link\":\"[]\",\"Name\":\"tsc_dev\",\"Resource.service.instance.id\":\"b17469f1-7243-4d4c-a921-76b84f358d3e\",\"Resource.service.name\":\"masa-tsc-service-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SpanId\":\"680ec468b36c6d80\",\"TraceId\":\"894c8d019af8f422e660581da8d1f165\",\"TraceStatus\":0}";
        string strMapping = "{\"mappings\":{\"properties\":{\"@timestamp\":{\"type\":\"date\"},\"Attributes\":{\"properties\":{\"db\":{\"properties\":{\"method\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"statement\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"statement_type\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"system\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"url\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"enduser\":{\"properties\":{\"id\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"nick_name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"exception\":{\"properties\":{\"message\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"stacktrace\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"host\":{\"properties\":{\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"http\":{\"properties\":{\"client_ip\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"flavor\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"host\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"method\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"request_content_body\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"request_content_length\":{\"type\":\"long\"},\"request_content_type\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"response_content_length\":{\"type\":\"long\"},\"response_content_type\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"scheme\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"status_code\":{\"type\":\"long\"},\"target\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"url\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"user_agent\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"net\":{\"properties\":{\"peer\":{\"properties\":{\"ip\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"port\":{\"type\":\"long\"}}}}},\"peer\":{\"properties\":{\"service\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}}}},\"EndTimestamp\":{\"type\":\"date\"},\"Kind\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Link\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"ParentSpanId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Resource\":{\"properties\":{\"service\":{\"properties\":{\"instance\":{\"properties\":{\"id\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"namespace\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"version\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"telemetry\":{\"properties\":{\"sdk\":{\"properties\":{\"language\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"version\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}}}}}},\"SpanId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"TraceId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"TraceStatus\":{\"type\":\"long\"}}}}";

        var httpClient = new HttpClient() { BaseAddress = new Uri(StaticConfig.HOST) };
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = new Uri($"/{StaticConfig.TRACE_INDEX_NAME}", UriKind.Relative) });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Put, RequestUri = new Uri($"/{StaticConfig.TRACE_INDEX_NAME}", UriKind.Relative), Content = new StringContent(strMapping, Encoding.UTF8, "application/json") });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri($"/{StaticConfig.TRACE_INDEX_NAME}/_doc", UriKind.Relative), Content = new StringContent(strHttpJson, Encoding.UTF8, "application/json") });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri($"/{StaticConfig.TRACE_INDEX_NAME}/_doc", UriKind.Relative), Content = new StringContent(strExceptionJson, Encoding.UTF8, "application/json") });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri($"/{StaticConfig.TRACE_INDEX_NAME}/_doc", UriKind.Relative), Content = new StringContent(strDbJson, Encoding.UTF8, "application/json") });
        Task.Delay(1000).Wait();
    }

    [TestInitialize]
    public void Initialize()
    {
        ServiceCollection services = new();
        services.Clear();
        services.AddElasticClientTrace(options =>
        {
            options.UseNodes(new string[] { StaticConfig.HOST });
        },
        callerOptions =>
        {
            callerOptions.BaseAddress = StaticConfig.HOST;
        },
        StaticConfig.TRACE_INDEX_NAME);
        var serviceProvider = services.BuildServiceProvider();
        _traceService = serviceProvider.GetRequiredService<ITraceService>();
    }

    [TestMethod]
    public async Task QueryTest()
    {
        Assert.IsNotNull(ElasticConst.Trace.Mappings.Value);

        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Sort =
                new FieldOrderDto
                {
                    Name = ElasticConst.ServiceName
                }
        };

        var result = await _traceService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());
    }

    [TestMethod]
    public async Task GetTest()
    {
        var result = await _traceService.GetAsync("277df6d0204f09fa63ff4ab896673455");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        bool isHttp = result.First().IsHttp(out var httpDto);
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
        bool isException = result.First().IsException(out var exceptionDto);
        Assert.IsTrue(isException);
        Assert.IsNotNull(exceptionDto);
        Assert.IsNotNull(exceptionDto.Message);

        result = await _traceService.GetAsync("894c8d019af8f422e660581da8d1f165");
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Any());
        bool isDatabase = result.First().IsDatabase(out var databaseDto);
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
            Name = ElasticConst.ServiceName,
            Type = AggregateTypes.Count
        };
        var result = await _traceService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var num = Convert.ToInt32(result);
        Assert.AreEqual(1, num);
    }
}
