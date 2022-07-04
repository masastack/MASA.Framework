// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Tsc.Tests;

[TestClass]
public class LogServiceTests
{
    [TestMethod]
    public async Task GetFieldsAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();

        var time = DateTime.Now;
        var query = new MetricLableValuesRequest
        {
            Match = "up",
            Start = time.AddMinutes(-15),
            End = time
        };
        var data = new string[]
        {
            "@timestamp",
            "container.instance.id",
            "container.instance.name",
            "Id"
        };
        callerProvider.Setup(provider => provider.GetAsync<IEnumerable<string>>(LogService.FIELD_URI, default)).ReturnsAsync(data).Verifiable();
        var client = new TscClient(callerProvider.Object);
        var result = await client.LogService.GetFieldsAsync();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetAggregationAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();

        var time = DateTime.Now;
        var query = new LogAggregationRequest
        {
            Start = time.AddMinutes(-15),
            End = time,
            FieldMaps = new FieldAggregationRequest[]
            {
                  new FieldAggregationRequest{
                       Name="container.instance.id",
                       AggType= LogAggTypes.Count,
                       Alias="count1"
                  },
                  new FieldAggregationRequest{
                       Name="container.instance.name",
                       AggType= LogAggTypes.Count,
                       Alias="count2"
                  }
              }
        };
        var data = new Dictionary<string, string>
        {
            {"count1","0" },
            { "count2","0"}
        };
        callerProvider.Setup(provider => provider.GetAsync<IEnumerable<KeyValuePair<string, string>>>(LogService.FIELD_URI, query, default)).ReturnsAsync(data).Verifiable();
        var client = new TscClient(callerProvider.Object);

        var result = await client.LogService.GetAggregationAsync(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetLatestAsyncTest()
    {
        var callerProvider = new Mock<ICallerProvider>();

        var time = DateTime.Now;
        var query = new LogLatestRequest
        {
            Start = time.AddMinutes(-15),
            End = time,
            IsDesc = true,
            Query = "\"term\": {\"Resource.service.name\": \"masa.tsc.api\"}"
        };

        var str = "{\"@timestamp\":\"2022-06-15T09:09:05.972899500Z\",\"Attributes.ProcessorName\":\"Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor.RetryByDataProcessor\",\"Attributes.exception.message\":\"SQLite Error 1: 'no such table: IntegrationEventLog'.\",\"Attributes.exception.type\":\"SqliteException\",\"Attributes.{OriginalFormat}\":\"Processor '{ProcessorName}' failed\",\"Body\":\"Processor 'Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Processor.RetryByDataProcessor' failed\",\"Resource.service.instance.id\":\"5d9d00e3-5bb0-40bc-bbb8-ef0b210f739d\",\"Resource.service.name\":\"masa.tsc.api\",\"Resource.service.namespace\":\"Development\",\"Resource.service.version\":\"0.1.0\",\"Resource.telemetry.sdk.language\":\"dotnet\",\"Resource.telemetry.sdk.name\":\"opentelemetry\",\"Resource.telemetry.sdk.version\":\"1.3.0.470\",\"SeverityNumber\":13,\"SeverityText\":\"Warning\",\"TraceFlags\":0}";
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(new JsonStringEnumConverter());
        var data = JsonSerializer.Deserialize<object>(str, options);
        callerProvider.Setup(provider => provider.GetAsync<object>(LogService.LATEST_URI, query, default)).ReturnsAsync(data).Verifiable();
        var client = new TscClient(callerProvider.Object);

        var result = await client.LogService.GetLatestAsync(query);
        Assert.IsNotNull(result);
    }
}
