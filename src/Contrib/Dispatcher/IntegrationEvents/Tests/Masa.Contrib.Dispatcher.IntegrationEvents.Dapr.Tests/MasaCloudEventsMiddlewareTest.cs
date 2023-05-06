// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Tests;

[TestClass]
public class MasaCloudEventsMiddlewareTest
{
    private Mock<IMultiEnvironmentSetter> _multiEnvironmentSetter;
    private Mock<IMultiTenantSetter> _multiTenantSetter;
    private IOptions<IsolationOptions> _isolationOptions;
    private MasaCloudEventsMiddleware _masaCloudEventsMiddleware;
    private int _timer;
    private StreamReader _nextRequestContent;

    [TestInitialize]
    public void Initialize()
    {
        _multiEnvironmentSetter = new Mock<IMultiEnvironmentSetter>();
        _multiEnvironmentSetter.Setup(setter => setter.SetEnvironment(It.IsAny<string>()));
        _multiTenantSetter = new Mock<IMultiTenantSetter>();
        _multiTenantSetter.Setup(setter => setter.SetTenant(It.IsAny<Tenant>()));
        var isolationOptions = new IsolationOptions()
        {
            MultiTenantIdName = "tenant",
            MultiEnvironmentName = "env"
        };
        _isolationOptions = Microsoft.Extensions.Options.Options.Create(isolationOptions);
        _masaCloudEventsMiddleware = new MasaCloudEventsMiddleware(CustomRequestDelegate, _isolationOptions);
    }

    [DataRow(null, false, null)]
    [DataRow("", false, null)]
    [DataRow("application/masacloudevents+json; charset=UTF-8", true, "UTF-8")]
    [DataRow("application/masacloudevents+json-custom", false, null)]
    [DataRow("application/masacloudevents+json", true, "UTF-8")]
    [DataRow("application/masacloudevents+json; charset=gb2312", true, "gb2312")]
    [DataRow("application/masacloudevents; charset=gb2312", false, null)]
    [DataRow("application/json", false, null)]
    [DataRow("application/xml", false, null)]
    [DataRow("multipart/form-data", false, null)]
    [DataTestMethod]
    public void TestMatchesContentType(string? contextType, bool expectedResult, string? expectedCharset)
    {
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                ContentType = contextType
            }
        };
        var result = _masaCloudEventsMiddleware.MatchesContentType(httpContext, out string? charset);

        Assert.AreEqual(expectedResult, result);
        Assert.AreEqual(expectedCharset, charset);
    }

    [TestMethod]
    public async Task TestProcessBodyAsync()
    {
        var jsonContent = JsonContent.Create(new
        {
            data = new RegisterUserIntegrationEvent()
            {
                Name = "masa"
            },
            isolation = new
            {
                env = "dev"
            }
        }, options: new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var httpContext = new DefaultHttpContext
        {
            Request =
            {
                Body = await jsonContent.ReadAsStreamAsync(),
                ContentType = "application/masacloudevents+json"
            },
        };
        var services = new ServiceCollection();
        services.AddSingleton(_ => _multiEnvironmentSetter.Object);
        services.AddSingleton(_ => _multiTenantSetter.Object);
        var serviceProvider = services.BuildServiceProvider();
        await _masaCloudEventsMiddleware.ProcessBodyAsync(httpContext, serviceProvider, "UTF-8");
        _multiEnvironmentSetter.Verify(x => x.SetEnvironment("dev"), Times.Exactly(1));
        _multiTenantSetter.Verify(x => x.SetTenant(It.IsAny<Tenant>()), Times.Exactly(0));
        Assert.AreEqual(1, _timer);

        var content = await _nextRequestContent.ReadToEndAsync();
        Assert.IsNotNull(content);
        var integrationEvent = JsonSerializer.Deserialize<RegisterUserIntegrationEvent>(content, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        Assert.IsNotNull(integrationEvent);
        Assert.AreEqual("masa", integrationEvent.Name);
    }

    [DataRow("1", 1, "test", 1)]
    [DataRow("1", 1, "", 0)]
    [DataRow("", 0, "test", 1)]
    [DataRow("", 0, "", 0)]
    [DataTestMethod]
    public void TestInitializeIsolation(
        string tenant,
        int expectedTimerByTenant,
        string environment,
        int expectedTimerByEnvironment)
    {
        var services = new ServiceCollection();
        services.AddSingleton(_ => _multiEnvironmentSetter.Object);
        services.AddSingleton(_ => _multiTenantSetter.Object);
        var serviceProvider = services.BuildServiceProvider();

        var dataInfo = new
        {
            isolation = new ExpandoObject()
        };

        if (!tenant.IsNullOrWhiteSpace())
        {
            dataInfo.isolation.TryAdd(_isolationOptions.Value.MultiTenantIdName, tenant);
        }
        if (!environment.IsNullOrWhiteSpace())
        {
            dataInfo.isolation.TryAdd(_isolationOptions.Value.MultiEnvironmentName, environment);
        }

        var dataStr = dataInfo.ToJson();
        var data = JsonSerializer.Deserialize<JsonElement>(dataStr);

        _masaCloudEventsMiddleware.InitializeIsolation(serviceProvider, data.GetProperty("isolation").EnumerateObject());

        _multiEnvironmentSetter.Verify(x => x.SetEnvironment(environment), Times.Exactly(expectedTimerByEnvironment));
        _multiTenantSetter.Verify(x => x.SetTenant(It.IsAny<Tenant>()), Times.Exactly(expectedTimerByTenant));
    }

    Task CustomRequestDelegate(HttpContext httpContext)
    {
        _nextRequestContent = new StreamReader(httpContext.Request.Body);
        _timer++;
        return Task.CompletedTask;
    }
}
