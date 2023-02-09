// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Tsc.Tests.Service;

[TestClass]
public class LogServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHandler = new();
    private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
    private const string HOST = "http://localhost";
    private ITscClient _client;
    private const string HTTP_CLIENT_NAME = "masa.contrib.stacksdks.tsc";

    [TestInitialize]
    public void Initialized()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddSingleton(_httpClientFactory.Object);
        var httpClient = new HttpClient(_mockHandler.Object)
        {
            BaseAddress = new Uri(HOST)
        };
        _httpClientFactory.Setup(factory => factory.CreateClient(HTTP_CLIENT_NAME)).Returns(httpClient);
        services.AddCaller(HTTP_CLIENT_NAME, builder =>
        {
            builder.UseHttpClient(options => options.BaseAddress = HOST);
        });
        var factory = services.BuildServiceProvider().GetRequiredService<ICallerFactory>();
        _client = new TscClient(factory.Create(HTTP_CLIENT_NAME));
    }

    [TestMethod]
    public async Task GetMappingFieldsAsyncTest()
    {
        var str = "[{\"name\":\"field1\",\"type\":\"text\"},{\"name\":\"field2\",\"type\":\"int\"}]";
        SetTestData(str);
        var result = await _client.LogService.GetMappingAsync();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetAggregationAsyncTest()
    {
        var time = DateTime.Now;
        var query = new SimpleAggregateRequestDto
        {
            Start = time.AddMinutes(-15),
            Keyword = "keyword",
            End = time,
            Name = "name",
            Type = AggregateTypes.Count
        };
        var str = "10";
        SetTestData(str);
        var result = await _client.LogService.GetAggregationAsync<string>(query);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetLatestAsyncTest()
    {
        var time = DateTime.Now;
        var query = new LogLatestRequest
        {
            Start = time.AddMinutes(-15),
            End = time,
            IsDesc = true,
            Query = "\"term\": {\"Resource.service.name\": \"masa.tsc.api\"}"
        };

        Assert.IsNotNull(query.Query);
        Assert.IsTrue(query.Start > DateTime.MinValue);
        Assert.IsTrue(query.End > DateTime.MinValue);
        Assert.IsTrue(query.IsDesc);

        var str =
            "{\"timestamp\":\"2022-11-15T07:01:28.2196126Z\",\"traceFlags\":0,\"severityText\":\"Information\",\"severityNumber\":9,\"body\":\"Request finished HTTP/2 GET https://localhost:18012/_blazor?id=_GmR6JVGGEuWA8wo5_vEgg&_=1668495688201 text/plain;charset=UTF-8 - - 200 1203 application/octet-stream 11.1170ms\",\"resource\":{\"service.instance.id\":\"57f5a1db-e0de-434d-aceb-45eb53a2efc8\",\"service.name\":\"masa-tsc-web-admin\",\"service.namespace\":\"Development\",\"service.version\":\"0.1.0\",\"telemetry.sdk.language\":\"dotnet\",\"telemetry.sdk.name\":\"opentelemetry\",\"telemetry.sdk.version\":\"1.3.0.519\"},\"attributes\":{\"dotnet.ilogger.category\":\"Microsoft.AspNetCore.Hosting.Diagnostics\"}}";
        SetTestData(str);
        var result = await _client.LogService.GetLatestAsync(query);
        Assert.IsNotNull(result);
    }

    private void SetTestData(string result, HttpStatusCode httpStatusCode = HttpStatusCode.OK)
    {
        _mockHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = httpStatusCode,
                Content = new StringContent(result)
            });
    }
}
