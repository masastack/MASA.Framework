// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using System.Net.Http.Json;

namespace Masa.Utils.Caller.Tests;

[TestClass]
public class HttpClientCallerTest
{
    [DataTestMethod]
    [DataRow("https://github.com/", "/check/healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com", "/check/healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com", "check/healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com/check", "healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com/check/", "healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com/check/", "/healthy", "https://github.com/check/healthy")]
    [DataRow("https://github.com/check/", "/healthy?date=1650465417", "https://github.com/check/healthy?date=1650465417")]
    [DataRow("https://github.com/check/", "healthy?date=1650465417", "https://github.com/check/healthy?date=1650465417")]
    [DataRow("https://github.com/check", "healthy?date=1650465417", "https://github.com/check/healthy?date=1650465417")]
    [DataRow("https://github.com", "https://github.com/check/healthy?date=1650465417", "https://github.com/check/healthy?date=1650465417")]
    [DataRow("https://github.com", "", "")]
    [DataRow("http://github.com", "", "")]
    [DataRow("/v1/check", "healthy", "/v1/check/healthy")]
    [DataRow("/v1/check/", "healthy", "/v1/check/healthy")]
    [DataRow("/v1/check/", "/healthy", "/v1/check/healthy")]
    [DataRow("/v1/check/", "/healthy", "/v1/check/healthy")]
    [DataRow("/v1/check/", "https://github.com/check/healthy?date=1650465417", "https://github.com/check/healthy?date=1650465417")]
    [DataRow("", "healthy", "healthy")]
    [DataRow("", "/healthy?id=1", "/healthy?id=1")]
    public void TestGetRequestUri(string prefix, string methods, string result)
    {
        var services = new ServiceCollection();
        services.AddCaller(opt => opt.UseHttpClient());
        var serviceProvider = services.BuildServiceProvider();
        var provider = new CustomHttpClientCallerProvider(serviceProvider, string.Empty, prefix);
        Assert.IsTrue(provider.GetResult(methods) == result);
    }

    [TestMethod]
    public async Task TestRequestDataIsXmlAsync()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITypeConvertProvider, DefaultTypeConvertProvider>();
        services.AddSingleton<IRequestMessage, XmlRequestMessage>();
        services.AddSingleton<IResponseMessage, DefaultXmlResponseMessage>();
        Mock<IHttpClientFactory> httpClientFactory = new();
        var handlerMock = new Mock<HttpMessageHandler>();
        var magicHttpClient = new System.Net.Http.HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
        var response = new BaseResponse("success");
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(XmlUtils.Serializer(response))
            })
            .Verifiable();

        httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(magicHttpClient);
        services.AddSingleton(httpClientFactory.Object);
        var serviceProvider = services.BuildServiceProvider();
        string name = "<Customize-Alias>";
        string prefix = "<Replace-Your-Service-Prefix>";
        var httpClientCallerProvider = new HttpClientCallerProvider(serviceProvider, name, prefix);

        var res = await httpClientCallerProvider.PostAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        Assert.IsNotNull(res);
        Assert.IsTrue(res.Code == response.Code);
    }

    [TestMethod]
    public async Task TestRequestMessageReturnOnceAsync()
    {
        var services = new ServiceCollection();
        RegisterUser registerUser = new RegisterUser("Jim", "123456");

        services.AddSingleton<ITypeConvertProvider, DefaultTypeConvertProvider>();
        Mock<IRequestMessage> requestMessage = new();
        requestMessage.Setup(req => req.ProcessHttpRequestMessageAsync(It.IsAny<HttpRequestMessage>()))
            .ReturnsAsync(new HttpRequestMessage(HttpMethod.Post, "Hello")).Verifiable();
        requestMessage.Setup(req => req.ProcessHttpRequestMessageAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<object>()))
            .ReturnsAsync(new HttpRequestMessage(HttpMethod.Post, "Hello")
            {
                Content = JsonContent.Create(registerUser)
            }).Verifiable();
        services.AddSingleton(_ => requestMessage.Object);
        services.AddSingleton<IResponseMessage, DefaultXmlResponseMessage>();
        Mock<IHttpClientFactory> httpClientFactory = new();
        var handlerMock = new Mock<HttpMessageHandler>();
        var magicHttpClient = new System.Net.Http.HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5000")
        };
        var response = new BaseResponse("success");
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(XmlUtils.Serializer(response))
            })
            .Verifiable();

        httpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(magicHttpClient);
        services.AddSingleton(httpClientFactory.Object);
        var serviceProvider = services.BuildServiceProvider();
        string name = "<Customize-Alias>";
        string prefix = "<Replace-Your-Service-Prefix>";
        var httpClientCallerProvider = new HttpClientCallerProvider(serviceProvider, name, prefix);

        var res = await httpClientCallerProvider.PostAsync<BaseResponse>("Hello", registerUser);
        Assert.IsNotNull(res);
        Assert.IsTrue(res.Code == response.Code);
        requestMessage.Verify(r => r.ProcessHttpRequestMessageAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<object>()), Times.Once);
    }
}
