// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Integrated.Tests;

[TestClass]
public class DomainEventTest
{
    private IServiceProvider _serviceProvider;

    [TestInitialize]
    public void Initialize()
    {
        IServiceCollection services = new ServiceCollection();
        services.Configure<MasaDbConnectionOptions>(options =>
        {
            options.ConnectionStrings = new ConnectionStrings()
            {
                DefaultConnection = $"Data Source=test_{Guid.NewGuid()}"
            };
        });
        services.AddDomainEventBus(dispatchOptions =>
        {
            dispatchOptions
                .UseIntegrationEventBus<IntegrationEventLogService>(options =>
                {
                    options.UseDapr();
                    options.UseEventLog<CustomDbContext>();
                })
                .UseEventBus()
                .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseSqlite())
                .UseRepository<CustomDbContext>();
        });
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task TestInsertAsyncReturnUserNameEqualTom()
    {
        var services = new ServiceCollection();
        services.AddDomainEventBus(dispatcherOptions =>
        {
            dispatcherOptions
                .UseIntegrationEventBus<IntegrationEventLogService>(options =>
                {
                    options.UseDapr();
                    options.UseEventLog<CustomDbContext>();
                })
                .UseEventBus()
                .UseUoW<CustomDbContext>(dbOptions => dbOptions.UseTestSqlite($"data source=test-{Guid.NewGuid()}"))
                .UseRepository<CustomDbContext>();
        });
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        var @event = new RegisterUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = "Jim"
        };
        await eventBus.PublishAsync(@event);

        var user = await dbContext.Set<User>().Where(u => u.Id == @event.Id).AsNoTracking().FirstOrDefaultAsync();
        Assert.IsNotNull(user);
        Assert.IsTrue(user.Name == "Tom2");
    }

    [TestMethod]
    public async Task TestPublishMultiCommandReturnDataIs2()
    {
        var dbContext = _serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

        var @event = new RegisterUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = "Jim1",
            IsSendDomainEvent = false
        };
        await eventBus.PublishAsync(@event);

        @event = new RegisterUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = "Jim2",
            IsSendDomainEvent = false
        };
        await eventBus.PublishAsync(@event);

        Assert.IsTrue(dbContext.Set<User>().Count() == 2);
    }

    [TestMethod]
    public async Task TestPublishMultiCommandReturnDataIs1()
    {
        var dbContext = _serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var eventBus = _serviceProvider.GetRequiredService<IEventBus>();

        var @event = new RegisterUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = "error",
            IsSendDomainEvent = false
        };
        await Assert.ThrowsExceptionAsync<Exception>(async () => await eventBus.PublishAsync(@event), "custom exception");

        @event = new RegisterUserEvent()
        {
            Id = Guid.NewGuid(),
            Name = "Jim2",
            IsSendDomainEvent = false
        };
        await eventBus.PublishAsync(@event);

        Assert.IsTrue(dbContext.Set<User>().Count() == 1);
    }
}
