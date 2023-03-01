// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Middleware.Tests;

[TestClass]
public class EventMiddlewareTest
{
    private IServiceProvider _serviceProvider;
    private readonly IEventBus _eventBus;

    public EventMiddlewareTest()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddMasaIdentity(options =>
        {
            options.Mapping(nameof(MasaUser.Account), "ACCOUNT");
        });

        Mock<IConfigurationApiClient> dccClient = new();
        var configs = new Dictionary<string, string>()
        {
            { MasaStackConfigConstant.IS_DEMO, builder.Configuration.GetValue<bool>(MasaStackConfigConstant.IS_DEMO).ToString() }
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
        builder.Services.AddTestEventBus(new Assembly[1] { Assembly.GetExecutingAssembly() }, ServiceLifetime.Scoped);
        builder.Services.AddStackMiddleware();

        _serviceProvider = builder.Services.BuildServiceProvider();
        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
        httpContextAccessor.HttpContext = new DefaultHttpContext()
        {
            User = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new(new List<Claim>()
                {
                    new(ClaimType.DEFAULT_USER_ID, "1"),
                    new("ACCOUNT", "guest")
                })
            })
        };

        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [TestMethod]
    public void TestCommand()
    {
        var command = new TestCommand();
        Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await _eventBus.PublishAsync(command), "DISABLED_OPERATE");
    }

    [TestMethod]
    public async Task TestAllowCommand()
    {
        var command = new TestAllowCommand();
        await _eventBus.PublishAsync(command);
        Assert.AreEqual(1, command.Count);
    }

    [TestMethod]
    public async Task TestQuery()
    {
        var query = new TestQuery();
        await _eventBus.PublishAsync(query);
        Assert.IsTrue(query.Result);
    }
}
