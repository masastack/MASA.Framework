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
        var mapping = StaticConfig.GetJson("Mapping:log");
        var dataJson = StaticConfig.GetJson("Init:log");
        var httpClient = new HttpClient() { BaseAddress = new Uri(StaticConfig.HOST) };
        httpClient.Send(StaticConfig.CreateMessage(StaticConfig.LOG_INDEX_NAME, HttpMethod.Delete));
        httpClient.Send(StaticConfig.CreateMessage(StaticConfig.LOG_INDEX_NAME, HttpMethod.Put, mapping));
        httpClient.Send(StaticConfig.CreateMessage($"{StaticConfig.LOG_INDEX_NAME}/_doc", HttpMethod.Post, dataJson));
        Task.Delay(1000).Wait();
    }

    [TestInitialize]
    public void Initialize()
    {
        ServiceCollection services = new();
        services.Clear();
        services.AddElasticClientLog(new string[] { StaticConfig.HOST }, StaticConfig.LOG_INDEX_NAME);
        var serviceProvider = services.BuildServiceProvider();
        _logService = serviceProvider.GetRequiredService<ILogService>();
    }

    [TestMethod]
    public async Task MappingTest()
    {
        var mappings = await _logService.GetMappingAsync();
        Assert.IsNotNull(mappings);
        Assert.IsTrue(mappings.Any());
        Assert.IsTrue(mappings.Any(m => m.Name == "@timestamp"));
    }

    [TestMethod]
    public async Task QueryEqualTest()
    {
        Assert.IsNotNull(ElasticConstant.Log.Mappings.Value);
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name="Attributes.Name", Type= ConditionTypes.Equal, Value="UserAuthorizationFailed" }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryNotEqualTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name="Attributes.Name", Type= ConditionTypes.NotEqual, Value="UserAuthorizationFailed" }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryKeywordTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Keyword = "requirements"
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryGreaterTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                new FieldConditionDto{ Name="SeverityNumber",Type= ConditionTypes.GreatEqual,Value=8 },
                new FieldConditionDto{ Name="SeverityNumber",Type= ConditionTypes.Less,Value=10 },
            }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryInTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConstant.SpanId, Type= ConditionTypes.In, Value=new string[]{ "1c495129b86de343", "aaaaaaa" } }
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());

        query.Conditions.First().Type = ConditionTypes.NotIn;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryRegexTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConstant.ServiceName, Type= ConditionTypes.Regex, Value="masa*"}
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());

        query.Conditions.First().Type = ConditionTypes.NotRegex;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Result.Any());
    }

    [TestMethod]
    public async Task QueryExistsTest()
    {
        var query = new BaseRequestDto
        {
            Page = 1,
            PageSize = 10,
            Conditions = new FieldConditionDto[] {
                 new FieldConditionDto{ Name=ElasticConstant.ParentId, Type= ConditionTypes.Exists}
             }
        };

        var result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsFalse(result.Result.Any());

        query.Conditions.First().Type = ConditionTypes.NotExists;
        result = await _logService.ListAsync(query);
        Assert.IsNotNull(result);
        Assert.IsTrue(result.Result.Any());
    }

    [TestMethod]
    public async Task AggCount()
    {
        var query = new SimpleAggregateRequestDto
        {
            Service = "masa-tsc-web-admin",
            Name = ElasticConstant.ServiceName,
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
    public async Task AggGroupByTest()
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Service = "masa-tsc-web-admin",
            Name = ElasticConstant.ServiceInstance,
            Type = AggregateTypes.GroupBy
        };

        var result = await _logService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var data = (IEnumerable<string>)result;
        Assert.IsTrue(data.Any());
    }

    [TestMethod]
    public async Task AggDateHistogramTest()
    {
        var query = new SimpleAggregateRequestDto
        {
            MaxCount = 10,
            Service = "masa-tsc-web-admin",
            Name = ElasticConstant.Log.Timestamp,
            Interval = "5m"
        };

        var result = await _logService.AggregateAsync(query);
        Assert.IsNull(result);

        query.Type = AggregateTypes.DateHistogram;
        result = await _logService.AggregateAsync(query);
        Assert.IsNotNull(result);
        var data = (IEnumerable<KeyValuePair<double, long>>)result;
        Assert.IsTrue(data.Any());
    }
}
