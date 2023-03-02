// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware.Tests;

[TestClass]
public class RequestMiddlewareTest
{
    private IServiceProvider _serviceProvider;

    public RequestMiddlewareTest()
    {
        var builder = WebApplication.CreateBuilder();

        Mock<IConfigurationApiClient> dccClient = new();
        var configuration = builder.Configuration;
        var configs = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.VERSION, configuration.GetValue<string>(MasaStackConfigConstant.VERSION) },
            { MasaStackConfigConstant.IS_DEMO, configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() },
            { MasaStackConfigConstant.DOMAIN_NAME, configuration.GetValue<string>(MasaStackConfigConstant.DOMAIN_NAME) },
            { MasaStackConfigConstant.NAMESPACE, configuration.GetValue<string>(MasaStackConfigConstant.NAMESPACE) },
            { MasaStackConfigConstant.TLS_NAME, configuration.GetValue<string>(MasaStackConfigConstant.TLS_NAME) },
            { MasaStackConfigConstant.CLUSTER, configuration.GetValue<string>(MasaStackConfigConstant.CLUSTER) },
            { MasaStackConfigConstant.OTLP_URL, configuration.GetValue<string>(MasaStackConfigConstant.OTLP_URL) },
            { MasaStackConfigConstant.REDIS, configuration.GetValue<string>(MasaStackConfigConstant.REDIS) },
            { MasaStackConfigConstant.CONNECTIONSTRING, configuration.GetValue<string>(MasaStackConfigConstant.CONNECTIONSTRING) },
            { MasaStackConfigConstant.MASA_SERVER, configuration.GetValue<string>(MasaStackConfigConstant.MASA_SERVER) },
            { MasaStackConfigConstant.MASA_UI, configuration.GetValue<string>(MasaStackConfigConstant.MASA_UI) },
            { MasaStackConfigConstant.ELASTIC, configuration.GetValue<string>(MasaStackConfigConstant.ELASTIC) },
            { MasaStackConfigConstant.ENVIRONMENT, configuration.GetValue<string>(MasaStackConfigConstant.ENVIRONMENT) },
            { MasaStackConfigConstant.ADMIN_PWD, configuration.GetValue<string>(MasaStackConfigConstant.ADMIN_PWD) },
            { MasaStackConfigConstant.DCC_SECRET, configuration.GetValue<string>(MasaStackConfigConstant.DCC_SECRET) }
        };
        dccClient.Setup(aa => aa.GetAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<Action<Dictionary<string, string>>>()!))
            .ReturnsAsync(configs);

        builder.Services.AddSingleton<IMasaStackConfig>(serviceProvider =>
        {
            return new MasaStackConfig(dccClient.Object, configs);
        });

        builder.Services.AddStackMiddleware();

        _serviceProvider = builder.Services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task TestRequestMiddleware()
    {
        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.Response.Body = new MemoryStream();
        defaultContext.Request.Path = "/";

        var requestMiddleware = _serviceProvider.GetRequiredService<DisabledRequestMiddleware>();
        await requestMiddleware.InvokeAsync(defaultContext, (innerHttpContext) =>
        {
            innerHttpContext.Response.WriteAsync("content");
            return Task.CompletedTask;
        });

        //Don't forget to rewind the stream
        defaultContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var body = new StreamReader(defaultContext.Response.Body).ReadToEnd();

        Assert.AreEqual("content", body);
    }

    [TestMethod]
    public void TestDisabledRequestMiddleware()
    {
        DefaultHttpContext defaultContext = new DefaultHttpContext();
        defaultContext.SetEndpoint(new Endpoint(c => Task.CompletedTask, new EndpointMetadataCollection(new DisabledRouteAttribute()), "myapp"));

        var requestMiddleware = _serviceProvider.GetRequiredService<DisabledRequestMiddleware>();

        Assert.ThrowsExceptionAsync<UserFriendlyException>(async () =>
        await requestMiddleware.InvokeAsync(defaultContext, (innerHttpContext) =>
        {
            return Task.CompletedTask;
        }));

        //Assert
        var endpoint = defaultContext.GetEndpoint();
        Assert.IsNotNull(endpoint);
    }
}
