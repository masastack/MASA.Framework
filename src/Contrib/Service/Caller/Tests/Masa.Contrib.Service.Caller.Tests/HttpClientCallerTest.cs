// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

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
        var caller = new CustomHttpClientCaller(serviceProvider, string.Empty, prefix);
        Assert.IsTrue(caller.GetResult(methods) == result);
    }

    [TestMethod]
    public async Task TestRequestDataIsXmlAsync()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ITypeConvertor, DefaultTypeConvertor>();
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
        string name = "<Custom-Alias>";
        string prefix = "<Replace-Your-Service-Prefix>";
        var caller = new HttpClientCaller(serviceProvider, name, prefix);

        var res = await caller.PostAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        Assert.IsNotNull(res);
        Assert.IsTrue(res.Code == response.Code);
    }

    [TestMethod]
    public async Task TestRequestMessageReturnOnceAsync()
    {
        var services = new ServiceCollection();
        RegisterUser registerUser = new RegisterUser("Jim", "123456");

        services.AddSingleton<ITypeConvertor, DefaultTypeConvertor>();
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
        string name = "<Custom-Alias>";
        string prefix = "<Replace-Your-Service-Prefix>";
        var caller = new HttpClientCaller(serviceProvider, name, prefix);

        var res = await caller.PostAsync<BaseResponse>("Hello", registerUser);
        Assert.IsNotNull(res);
        Assert.IsTrue(res.Code == response.Code);
        requestMessage.Verify(r => r.ProcessHttpRequestMessageAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<object>()), Times.Once);
    }

    [TestMethod]
    public async Task TestResponseAsync()
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null")
        };
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());
        Assert.IsNull(await defaultResponseMessage.ProcessResponseAsync<object?>(httpResponseMessage));

        Assert.IsNull(await defaultResponseMessage.ProcessResponseAsync<int?>(httpResponseMessage));

        Assert.IsNull(await defaultResponseMessage.ProcessResponseAsync<Guid?>(httpResponseMessage));

        Assert.IsNull(await defaultResponseMessage.ProcessResponseAsync<DateTime?>(httpResponseMessage));

        Assert.IsTrue(await defaultResponseMessage.ProcessResponseAsync<int>(httpResponseMessage) == 0);
        Assert.IsTrue(await defaultResponseMessage.ProcessResponseAsync<Guid>(httpResponseMessage) == default);
        Assert.IsTrue(await defaultResponseMessage.ProcessResponseAsync<DateTime>(httpResponseMessage) == default);
    }

    [TestMethod]
    public async Task TestResponseIsStringArrayAsync()
    {
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());
        var result = await defaultResponseMessage.ProcessResponseAsync<List<string>>(GetHttpResponseMessage());
        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("test", result[0]);

        var result2 = (await defaultResponseMessage.ProcessResponseAsync<string[]?>(GetHttpResponseMessage()))?.ToList();
        Assert.IsNotNull(result2);
        Assert.AreEqual(1, result2.Count);
        Assert.AreEqual("test", result2[0]);

        HttpResponseMessage GetHttpResponseMessage()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<string>()
                {
                    "test"
                }))
            };
        }
    }

    [TestMethod]
    public async Task TestResponseIsIntArrayAsync()
    {
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());
        var result = await defaultResponseMessage.ProcessResponseAsync<List<int>>(GetHttpResponseMessage());
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual(1, result[0]);
        Assert.AreEqual(2, result[1]);

        var result2 = (await defaultResponseMessage.ProcessResponseAsync<int[]?>(GetHttpResponseMessage()))?.ToList();
        Assert.IsNotNull(result2);
        Assert.AreEqual(2, result2.Count);
        Assert.AreEqual(1, result2[0]);
        Assert.AreEqual(2, result2[1]);

        HttpResponseMessage GetHttpResponseMessage()
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new List<int>()
                {
                    1, 2
                }))
            };
        }
    }

    [TestMethod]
    public async Task TestResponseIsIntAsync()
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(1.ToString())
        };
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());

        var res = await defaultResponseMessage.ProcessResponseAsync<int>(httpResponseMessage);
        Assert.IsNotNull(res);
        Assert.IsTrue(res == 1);

        var res2 = await defaultResponseMessage.ProcessResponseAsync<int?>(httpResponseMessage);
        Assert.IsNotNull(res2);
        Assert.IsTrue(res2 == 1);
    }

    [TestMethod]
    public async Task TestResponseIsGuidAsync()
    {
        Guid id = Guid.NewGuid();
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(id.ToString())
        };
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());

        var res = await defaultResponseMessage.ProcessResponseAsync<Guid?>(httpResponseMessage);
        Assert.IsNotNull(res);
        Assert.IsTrue(res == id);

        var res2 = await defaultResponseMessage.ProcessResponseAsync<Guid>(httpResponseMessage);
        Assert.IsNotNull(res2);
        Assert.IsTrue(res2 == id);
    }

    [TestMethod]
    public async Task TestResponseIsDateTimeAsync()
    {
        var date = DateTime.Now;
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(date.ToString("yyyy-MM-dd HH:mm:ss"))
        };
        var services = new ServiceCollection();
        services.Configure<CallerFactoryOptions>(option =>
        {
            option.JsonSerializerOptions = new JsonSerializerOptions();
        });
        var serviceProvider = services.BuildServiceProvider();
        var defaultResponseMessage = new JsonResponseMessage(serviceProvider.GetRequiredService<IOptions<CallerFactoryOptions>>());

        var res = await defaultResponseMessage.ProcessResponseAsync<DateTime?>(httpResponseMessage);
        Assert.IsNotNull(res);
        Assert.IsTrue(res.Value.ToString("yyyy-MM-dd HH:mm:ss") == date.ToString("yyyy-MM-dd HH:mm:ss"));

        var res2 = await defaultResponseMessage.ProcessResponseAsync<DateTime>(httpResponseMessage);
        Assert.IsNotNull(res2);
        Assert.IsTrue(res2.ToString("yyyy-MM-dd HH:mm:ss") == date.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}
