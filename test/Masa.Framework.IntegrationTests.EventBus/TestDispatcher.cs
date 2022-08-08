// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Framework.IntegrationTests.EventBus.Infrastructure;
using Masa.Framework.IntegrationTests.EventBus.Infrastructure.Extensions;
using Masa.Framework.IntegrationTests.EventBus.Infrastructure.Middleware;

namespace Masa.Framework.IntegrationTests.EventBus;

[TestClass]
public class TestDispatcher : TestBase
{
    [TestMethod]
    public async Task TestEventReturnNotUseTranscationAsync()
    {
        var serviceProvider = ServiceProvider;
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var @event = new RegisterUserEvent()
        {
            Name = "Jim",
            Age = 18
        };
        await eventBus.PublishAsync(@event);

        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(dbContext.Set<User>().Count() == 1);
    }

    [TestMethod]
    public async Task TestCommandReturnUseTransacationAndTimeEqual1Async()
    {
        var serviceProvider = ServiceProvider;
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var command = new RegisterUserCommand()
        {
            Name = "Jim",
            Age = 18
        };
        await eventBus.PublishAsync(command);

        Assert.IsTrue(RecordMiddleware<RegisterUserCommand>.Time == 1);
        Assert.IsTrue(RecordMiddleware<CheckUserQuery>.Time == 0);

        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        Assert.IsTrue(dbContext.Set<User>().Count() == 1);
    }

    [TestMethod]
    public async Task TestQueryReturnNameIsRequiredOnCheckUserQueryAsync()
    {
        var serviceProvider = ServiceProvider;
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Set<User>().AddAsync(new User()
        {
            Name = "Tom",
            Age = 18
        });
        await dbContext.SaveChangesAsync();

        var query = new UserAgeQuery()
        {
            Name = "Jim",
        };
        await eventBus.PublishAsync(query);
        Assert.IsTrue(query.Result == default);

        Assert.IsTrue(RecordMiddleware<UserAgeQuery>.Time == 1);
        Assert.IsTrue(RecordMiddleware<CheckUserQuery>.Time == 0);
    }

    [TestMethod]
    public async Task TestIntegrationEventAndNotUseEventLogServiceReturnNoError()
    {
        var services = new ServiceCollection();
        services.AddIntegrationEventBus(option => option.UseTestPub().UseEventBus());
        var serviceProvider = services.BuildServiceProvider();

        var integrationEventBus = serviceProvider.GetRequiredService<IIntegrationEventBus>();
        await integrationEventBus.PublishAsync(new AddGoodsIntegrationEvent()
        {
            Name = "Apple",
            Count = 1,
            Id = Guid.NewGuid(),
            Price = 9.9m,
        });
    }
}
