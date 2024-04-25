// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Ddd.Domain.Events;
using Masa.Framework.IntegrationTests.EventBus.Application;
using System.Linq;
using System.Reflection;

namespace Masa.Framework.IntegrationTests.EventBus;

[TestClass]
public class TestDispatcher : TestBase
{
    [TestMethod]
    public async Task TestEventReturnNotUseTransactionAsync()
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

        Assert.IsTrue(RecordEventMiddleware<RegisterUserCommand>.Time == 1);
        Assert.IsTrue(RecordEventMiddleware<CheckUserQuery>.Time == 0);

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

        Assert.IsTrue(RecordEventMiddleware<UserAgeQuery>.Time == 1);
        Assert.IsTrue(RecordEventMiddleware<CheckUserQuery>.Time == 0);
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

    [TestMethod]
    public async Task TestEventBusOnConcurrencyAsync()
    {
        var serviceProvider = ServiceProvider;
        var @event = new RegisterUserEvent()
        {
            Name = Guid.NewGuid().ToString(),
            Age = 18
        };
        var tasks = new ConcurrentBag<Task>();

        var testCount = 100;
        Parallel.For(1L, testCount + 1, i =>
        {
            tasks.Add(AddUserAsync(serviceProvider, @event));
        });

        await Task.WhenAll(tasks);

        var customizeDbContext = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<CustomDbContext>();
        var count = customizeDbContext.Set<User>().Count();
        Assert.IsTrue(count == testCount);
    }

    private async Task AddUserAsync(IServiceProvider serviceProvider, RegisterUserEvent @event)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestEntityCreatedEventAsync()
    {
        var serviceProvider = ServiceProvider;
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var domainEventBus = serviceProvider.GetRequiredService<IDomainEventBus>();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        var users = new List<User>();
        var testCount = 100;
        Parallel.For(1L, testCount + 1, i =>
        {
            users.Add(new User
            {
                Age = 18,
                Name = i.ToString()
            });
        });
        await dbContext.Set<User>().AddRangeAsync(users);
        await dbContext.SaveChangesAsync();
        await unitOfWork.SaveChangesAsync();
        await unitOfWork.CommitAsync();
        var count = dbContext.Set<User>().Count();
        Assert.IsTrue(count == testCount);
    }
}
