// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.Service.Caller.Tests;

[TestClass]
public class CallerTest
{
    private WebApplicationBuilder _builder = default!;

    [TestInitialize]
    public void EdgeDriverInitialize()
    {
        _builder = WebApplication.CreateBuilder();
    }

    [TestMethod]
    public async Task TestGetAsync()
    {
        _builder.Services.AddCaller(callerOptions =>
        {
            callerOptions.UseHttpClient(httpClientBuilder =>
            {
                httpClientBuilder.Configure = builder => builder.Timeout = TimeSpan.FromSeconds(3);
                httpClientBuilder.BaseAddress = "https://github.com/masastack";
            });
            callerOptions.DisableAutoRegistration = true;
        });
        _ = _builder.Build();
        var serviceProvider = _builder.Services.BuildServiceProvider();
        var githubCaller = serviceProvider.GetRequiredService<ICaller>();
        Assert.IsTrue(await GetAsync(githubCaller));
    }

    private async Task<bool> GetAsync(ICaller caller)
    {
        var res = await caller.GetAsync("");
        return res.IsSuccessStatusCode && res.StatusCode == HttpStatusCode.OK;
    }

    [TestMethod]
    public void TestConvertToDictionaryByDynamic()
    {
        var provider = new DefaultTypeConvertor();
        var dictionary = new Dictionary<string, string>
        {
            { "account", "jim" },
            { "age", "18" }
        };
        var request = new
        {
            account = "jim",
            age = 18
        };
        var result = provider.ConvertToDictionary(request);
        Assert.IsTrue(JsonSerializer.Serialize(result) == JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject()
    {
        var provider = new DefaultTypeConvertor();
        var query = new UserListQuery("Jim");
        var dictionary = new Dictionary<string, string>
        {
            { "name", query.Name }
        };
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(JsonSerializer.Serialize(result) == JsonSerializer.Serialize(dictionary));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject2()
    {
        var provider = new DefaultTypeConvertor();
        var query = new UserDetailQuery("Jim", "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 2);
        Assert.IsTrue(result["name"] == query.Name);
        Assert.IsTrue(result["Tags"] == JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject3()
    {
        var provider = new DefaultTypeConvertor();

        List<string> tags = null!;
        var query = new UserDetailQuery("Jim", tags);
        var result = provider.ConvertToDictionary(query);

        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["name"] == query.Name);
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject4()
    {
        var provider = new DefaultTypeConvertor();
        var query = new UserDetailQuery(null!, "Music", "Game");
        var result = provider.ConvertToDictionary(query);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Tags"] == JsonSerializer.Serialize(new List<string>()
        {
            "Music",
            "Game"
        }));
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject5()
    {
        var provider = new DefaultTypeConvertor();
        var dic = new Dictionary<string, string>()
        {
            { "Account", "Jim" }
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }

    [TestMethod]
    public void TestConvertToDictionaryByObject6()
    {
        var provider = new DefaultTypeConvertor();
        var dic = new List<KeyValuePair<string, string>>()
        {
            new("Account", "Jim")
        };
        var result = provider.ConvertToDictionary(dic);
        Assert.IsTrue(result.Count == 1);
        Assert.IsTrue(result["Account"] == "Jim");
    }

    [TestMethod]
    public void TestCustomCallerNameReturnNotNull()
    {
        var baseAddress = "http://www.github.com";
        var services = new ServiceCollection();
        services.AddCaller(options =>
        {
            options.UseHttpClient("gitee", httpClientBuilder =>
            {
                httpClientBuilder.BaseApi = "http://www.gitee.com";
            });
            options.UseHttpClient(httpClientBuilder =>
            {
                httpClientBuilder.BaseApi = baseAddress;
            });
            options.DisableAutoRegistration = true;
        });

        var caller = services.BuildServiceProvider().GetService<ICaller>();
        Assert.IsNotNull(caller);

        var httpClientCaller = ((HttpClientCaller)caller);
        System.Net.Http.HttpClient httpClient =
            (System.Net.Http.HttpClient)httpClientCaller.GetType()
                .GetField("_httpClient", BindingFlags.Instance | BindingFlags.NonPublic)!
                .GetValue(httpClientCaller)!;
        Assert.IsTrue(httpClient!.BaseAddress!.OriginalString == baseAddress);

    }

    [TestMethod]
    public void TestRepeatCallerNameReturnArgumentException()
    {
        var services = new ServiceCollection();
        services.AddCaller(options =>
        {
            options.UseHttpClient(httpClientBuilder =>
            {
                httpClientBuilder.BaseApi = "http://www.github.com";
            });
            options.DisableAutoRegistration = true;
        });
        services.AddCaller(options =>
        {
            options.UseHttpClient(httpClientBuilder =>
            {
                httpClientBuilder.BaseApi = "http://www.gitee.com";
            });
            options.DisableAutoRegistration = true;
        });

        Assert.ThrowsException<ArgumentException>(() => services.BuildServiceProvider().GetService<ICaller>());
    }


    [TestMethod]
    public void TestInitializationMasaHttpClientBuilderReturnEqual()
    {
        var masaHttpClientBuilder = new MasaHttpClientBuilder();
        Assert.IsTrue(masaHttpClientBuilder.Prefix == string.Empty);
        Assert.IsTrue(masaHttpClientBuilder.BaseAddress == string.Empty);
        Assert.IsTrue(masaHttpClientBuilder.Configure == null);


        masaHttpClientBuilder = new MasaHttpClientBuilder("http://www.github.com", _ =>
        {
        });
        Assert.IsTrue(masaHttpClientBuilder.Prefix == string.Empty);
        Assert.IsTrue(masaHttpClientBuilder.BaseAddress == "http://www.github.com");

        masaHttpClientBuilder = new MasaHttpClientBuilder("http://www.github.com", "api", _ =>
        {
        });
        Assert.IsTrue(masaHttpClientBuilder.Prefix == "api");
        Assert.IsTrue(masaHttpClientBuilder.BaseAddress == "http://www.github.com");
    }

    [TestMethod]
    public void TestNotUseCaller()
    {
        var services = new ServiceCollection();
        services.AddCaller();
        var serviceProvider = services.BuildServiceProvider();
        var callerFactory = serviceProvider.GetRequiredService<ICallerFactory>();
        Assert.ThrowsException<NotImplementedException>(() => callerFactory.Create(),
            "No default Caller found, you may need service.AddCaller()");
        Assert.ThrowsException<NotImplementedException>(() => callerFactory.Create("test"),
            string.Format("Please make sure you have used [{0}] Caller, it was not found", "test"));
    }

    [TestMethod]
    public async Task TestConfigRequestMessageByHttpClientCallerAsync()
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

        var excuteCount = 0;
        caller.ConfigRequestMessage(httpRequestMessage =>
        {
            excuteCount++;
            Assert.IsTrue(httpRequestMessage.RequestUri!.ToString().Contains($"{prefix}/Hello"));
            return Task.CompletedTask;
        });

        await caller.PostAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.GetAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.GetAsync<BaseResponse>("Hello?account=Jim&password=123456");
        await caller.PutAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.PatchAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.DeleteAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));

        Assert.IsTrue(excuteCount == 6);
    }

    [TestMethod]
    public async Task TestConfigRequestMessageByDaprClientCallerAsync()
    {
        var services = new ServiceCollection();
        services.AddOptions();
        services.AddSingleton<IRequestMessage, XmlRequestMessage>();
        services.AddSingleton<ITypeConvertor, DefaultTypeConvertor>();
        services.AddSingleton<IResponseMessage, DefaultXmlResponseMessage>();
        Mock<Dapr.Client.DaprClient> daprClient = new();
        var response = new BaseResponse("success");
        daprClient.Setup(client => client.CreateInvokeMethodRequest(It.IsAny<HttpMethod>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new HttpRequestMessage());
        daprClient.Setup(client => client.InvokeMethodWithResponseAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(XmlUtils.Serializer(response))
            });
        services.AddSingleton(daprClient.Object);
        var serviceProvider = services.BuildServiceProvider();
        string name = "<Custom-Alias>";
        string prefix = "<Replace-Your-Service-Prefix>";
        var caller = new DaprCaller(serviceProvider, name, prefix);
        var excuteCount = 0;
        caller.ConfigRequestMessage(httpRequestMessage =>
        {
            excuteCount++;
            return Task.CompletedTask;
        });

        await caller.PostAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.GetAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.GetAsync<BaseResponse>("Hello?account=Jim&password=123456");
        await caller.PutAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.PatchAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));
        await caller.DeleteAsync<BaseResponse>("Hello", new RegisterUser("Jim", "123456"));

        Assert.IsTrue(excuteCount == 6);
    }
}
