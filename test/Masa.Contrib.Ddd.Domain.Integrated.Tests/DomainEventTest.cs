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
                    options.UseEventLog<CustomizeDbContext>();
                })
                .UseEventBus()
                .UseUoW<CustomizeDbContext>(dbOptions => dbOptions.UseSqlite())
                .UseRepository<CustomizeDbContext>();
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
                    options.UseEventLog<CustomizeDbContext>();
                })
                .UseEventBus()
                .UseUoW<CustomizeDbContext>(dbOptions => dbOptions.UseTestSqlite($"data source=test-{Guid.NewGuid()}"))
                .UseRepository<CustomizeDbContext>();
        });
        var serviceProvider = services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomizeDbContext>();
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
        Assert.IsTrue(user.Name == "Tom");
    }
}
