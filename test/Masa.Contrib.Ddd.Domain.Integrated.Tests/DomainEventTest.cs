// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data;
using Masa.BuildingBlocks.Ddd.Domain.Events;
using Masa.Contrib.Ddd.Domain.Integrated.Tests.DomainEvents;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Masa.Contrib.Ddd.Domain.Entities.Tests;

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
            .UseDaprEventBus<IntegrationEventLogService>(options => options.UseEventLog<CustomizeDbContext>())
            .UseEventBus()
            .UseUoW<CustomizeDbContext>(dbOptions => dbOptions.UseSqlite())
            .UseRepository<CustomizeDbContext>();
        });
        _serviceProvider = services.BuildServiceProvider();
    }

    [TestMethod]
    public async Task TestSaveChangesReturnDomainEventBus()
    {
        var dbContext = _serviceProvider.GetRequiredService<CustomizeDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        var user = new User()
        {
            Name = "Jim"
        };
        user.AddDomainEvent(new AddUserIntegrationDomainEvent()
        {
            Name = user.Name
        });
        await dbContext.Set<User>().AddAsync(user);
        await dbContext.SaveChangesAsync();
        var domainEventBus = _serviceProvider.GetRequiredService<IDomainEventBus>();
        var fields = domainEventBus.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic).ToList();
        var field = fields.Where(field => field.Name == "_eventQueue").First();
        var eventQueue = (ConcurrentQueue<IDomainEvent>)field.GetValue(domainEventBus)!;
        Assert.IsTrue(eventQueue.Count == 1);
    }
}
