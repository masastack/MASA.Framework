// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Elasticseach.Tests.Service;

[TestClass]
public class LogServiceTests
{
    private ILogService _logService;
   
    [ClassInitialize]
    public static void InitializeLog(TestContext testContext)
    {
        string strJson = "{\"@timestamp\":\"2022-11-15T12:12:11.028116800Z\",\"Attributes.Id\":2,\"Attributes.Name\":\"UserAuthorizationFailed\",\"Attributes.Reason\":\"These requirements were not met:\\r\\nDenyAnonymousAuthorizationRequirement: Requires an authenticated user.\",\"Attributes.[Scope.0]:ParentId\":\"0000000000000000\",\"Attributes.[Scope.0]:SpanId\":\"1c495129b86de343\",\"Attributes.[Scope.0]:TraceId\":\"277df6d0204f09fa63ff4ab896673455\",\"Attributes.[Scope.1]:ConnectionId\":\"0HMM705VTO6BQ\",\"Attributes.[Scope.2]:RequestId\":\"0HMM705VTO6BQ:00000001\",\"Attributes.[Scope.2]:RequestPath\":\"/\",\"Attributes.dotnet.ilogger.category\":\"Microsoft.AspNetCore.Authorization.DefaultAuthorizationService\",\"Attributes.{OriginalFormat}\":\"Authorization failed. {Reason}\",\"Body\":\"Authorization failed. These requirements were not met:\\r\\nDenyAnonymousAuthorizationRequirement: Requires an authenticated user.\",\"Resource.service.instance.id\":\"9eaf1452-8b12-4368-928c-48f871c250fe\",\"Resource.service.name\":\"masa-tsc-web-admin\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.519\",\"SeverityNumber\":9,\"SeverityText\":\"Information\",\"SpanId\":\"1c495129b86de343\",\"TraceFlags\":1,\"TraceId\":\"277df6d0204f09fa63ff4ab896673455\"}";
        string strMapping = "{\"mappings\":{\"properties\":{\"@timestamp\":{\"type\":\"date\"},\"Attributes\":{\"properties\":{\"AuthenticationScheme\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"CommandType\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"ContentLength\":{\"type\":\"long\"},\"ContentType\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"ElapsedMilliseconds\":{\"type\":\"float\"},\"EndpointName\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"FullName\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Host\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Id\":{\"type\":\"long\"},\"Method\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Path\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"PathBase\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Protocol\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"QueryString\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Reason\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Scheme\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"StatusCode\":{\"type\":\"long\"},\"[Scope\":{\"properties\":{\"0]:ParentId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"0]:SpanId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"0]:TraceId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"1]:ConnectionId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"2]:RequestId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"2]:RequestPath\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"address\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"contentRoot\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"dotnet\":{\"properties\":{\"ilogger\":{\"properties\":{\"category\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}}}},\"envName\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"exception\":{\"properties\":{\"message\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"stacktrace\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"type\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"{OriginalFormat}\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"Body\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"Resource\":{\"properties\":{\"service\":{\"properties\":{\"instance\":{\"properties\":{\"id\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"namespace\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"version\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}},\"telemetry\":{\"properties\":{\"sdk\":{\"properties\":{\"language\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"name\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"version\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}}}}}},\"SeverityNumber\":{\"type\":\"long\"},\"SeverityText\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"SpanId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}},\"TraceFlags\":{\"type\":\"long\"},\"TraceId\":{\"type\":\"text\",\"fields\":{\"keyword\":{\"type\":\"keyword\",\"ignore_above\":256}}}}}}";
        var httpClient = new HttpClient() { BaseAddress = new Uri(StaticConfig.HOST) };
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Delete, RequestUri = new Uri($"/{StaticConfig.LOG_INDEX_NAME}", UriKind.Relative) });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Put, RequestUri = new Uri($"/{StaticConfig.LOG_INDEX_NAME}", UriKind.Relative), Content = new StringContent(strMapping, Encoding.UTF8, "application/json") });
        httpClient.Send(new HttpRequestMessage { Method = HttpMethod.Post, RequestUri = new Uri($"/{StaticConfig.LOG_INDEX_NAME}/_doc", UriKind.Relative), Content = new StringContent(strJson, Encoding.UTF8, "application/json") });
        Task.Delay(1000).Wait();
    }

    [TestInitialize]
    public void Initialize()
    {
        ServiceCollection services = new();
        services.AddElasticClientLog(new string[] { StaticConfig.HOST }, StaticConfig.LOG_INDEX_NAME);
        var serviceProvider = services.BuildServiceProvider();
        _logService = serviceProvider.GetRequiredService<ILogService>();
    }

    [TestMethod]
    public async Task MappingTest()
    {
        var mappings = await _logService.MappingAsync();
        Assert.IsNotNull(mappings);
        Assert.IsTrue(mappings.Any());
        Assert.IsTrue(mappings.Any(m => m.Name == "@timestamp"));
    }

    [TestMethod]
    public async Task QueryEqualTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name="Attributes.Name", Type= ConditionTypes.Equal, Value="UserAuthorizationFailed" }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryNotEqualTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name="Attributes.Name", Type= ConditionTypes.NotEqual, Value="UserAuthorizationFailed" }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryKeywordTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Keyword = "requirements"
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryGreaterTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{ Name="SeverityNumber",Type= ConditionTypes.GreatEqual,Value=8 },
                new FieldConditionDto{ Name="SeverityNumber",Type= ConditionTypes.Less,Value=10 },
            }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryInTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConst.SpanId, Type= ConditionTypes.In, Value=new string[]{ "1c495129b86de343", "aaaaaaa" } }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());

        query.Conditions.First().Type = ConditionTypes.NotIn;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryRegexTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConst.ServiceName, Type= ConditionTypes.Regex, Value="masa*"}
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());

        query.Conditions.First().Type = ConditionTypes.NotRegex;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Items.Any());
    }

    [TestMethod]
    public async Task QueryExistsTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            Size = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConst.ParentId, Type= ConditionTypes.Exists}
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Items.Any());

        query.Conditions.First().Type = ConditionTypes.NotExists;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Items.Any());
    }

    [TestMethod]
    public async Task AggCount()
    {
        var query = new SimpleAggregateRequestDto
        {
            Service = "masa-tsc-web-admin",
            Name = ElasticConst.ServiceName,
            Type = AggregateTypes.Count
        };
        var result = await _logService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var num = Convert.ToInt32(result);
        Assert.AreEqual(1, num);
    }

    [TestMethod]
    [DataRow(AggregateTypes.Avg)]
    [DataRow(AggregateTypes.Sum)]
    public async Task AggNumber(AggregateTypes type)
    {
        var query = new SimpleAggregateRequestDto
        {
            Service = "masa-tsc-web-admin",
            Name = "Attributes.Id",
            Type = type
        };
        var result = await _logService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var num = Convert.ToInt32(result);
        Assert.AreEqual(2, num);

        query.Name = "Attributes.Name";
        var notNumException = false;
        try
        {
            result = await _logService.AggregateAsync(query);
        }
        catch
        {
            notNumException = true;
        }
        Assert.IsTrue(notNumException);
    }

    [TestMethod]
    public async Task AggGroupBy()
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Service = "masa-tsc-web-admin",
            Name = ElasticConst.ServiceInstance,
            Type = AggregateTypes.GroupBy
        };

        var result = await _logService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var data = (IEnumerable<string>)result;
        Assert.IsTrue(data.Any());
    }
}
