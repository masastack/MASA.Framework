// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests;

[TestClass]
public class IntegrationEventLogServiceTest : TestBase
{
    [TestMethod]
    public async Task TestNullDbTransactionAsync()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<CustomDbContext>(builder => builder.UseTestSqlite(ConnectionString))
            .AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        IDispatcherOptions dispatcherOptions = CreateDispatcherOptions(services);
        dispatcherOptions.UseEventLog<CustomDbContext>();
        var serviceProvider = services.BuildServiceProvider();

        DbTransaction transaction = null!;
        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        await Assert.ThrowsExceptionAsync<MasaArgumentException>(async () => await eventLogService.SaveEventAsync(@event, transaction));
    }

    [TestMethod]
    public void TestNullServices()
    {
        var dispatcherOptions = CreateDispatcherOptions(null!);

        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            dispatcherOptions.UseEventLog<CustomDbContext>();
        });
    }

    [TestMethod]
    public void TestAddMultEventLog()
    {
        var services = new ServiceCollection();
        IDispatcherOptions dispatcherOptions = CreateDispatcherOptions(services);
        dispatcherOptions.UseEventLog<CustomDbContext>().UseEventLog<CustomDbContext>();
        Assert.IsTrue(services.Count(service => service.ImplementationType == typeof(IntegrationEventLogModelCreatingProvider)) == 1);
        Assert.IsTrue(services.Count(service => service.ServiceType == typeof(IntegrationEventLogContext)) == 1);
    }

    [TestMethod]
    public async Task TestRetrieveEventLogsPendingToPublishAsync()
    {
        var dispatcherOptions = CreateDispatcherOptions(new ServiceCollection());
        dispatcherOptions.UseEventLog<CustomDbContext>();
        dispatcherOptions.Services.AddMasaDbContext<CustomDbContext>(option => option.UseTestSqlite(Connection));
        dispatcherOptions.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        var serviceProvider = dispatcherOptions.Services.BuildServiceProvider();
        await serviceProvider.GetRequiredService<CustomDbContext>().Database.EnsureCreatedAsync();
        var logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        var list = await logService.RetrieveEventLogsPendingToPublishAsync(100);
        Assert.IsTrue(!list.Any());
    }

    [TestMethod]
    public async Task TestRetrieveEventLogsPendingToPublish2Async()
    {
        var response = await InitializeAsync();

        #region Initialization data

        var logs = await response.CustomDbContext.Set<IntegrationEventLog>().ToListAsync();
        response.CustomDbContext.Set<IntegrationEventLog>().RemoveRange(logs);

        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await response.CustomDbContext.Set<IntegrationEventLog>().AddAsync(new IntegrationEventLog(@event, Guid.NewGuid())
        {
            State = IntegrationEventStates.InProgress,
            ModificationTime = DateTime.UtcNow.AddSeconds(-120),
        });
        await response.CustomDbContext.SaveChangesAsync();

        #endregion

        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var list = (await logService.RetrieveEventLogsFailedToPublishAsync(100, 10, 60)).ToList();
        Assert.IsTrue(list.Count == 1);

        var eventLog = list.Select(log => log.Event).FirstOrDefault()!;
        Assert.IsTrue(eventLog.Equals(@event));
    }

    [TestMethod]
    public async Task TestRetrieveEventLogsFailedToPublishAsync()
    {
        var dispatcherOptions = CreateDispatcherOptions(new ServiceCollection());
        dispatcherOptions.UseEventLog<CustomDbContext>();
        dispatcherOptions.Services.AddMasaDbContext<CustomDbContext>(option => option.UseTestSqlite(Connection));
        dispatcherOptions.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        var serviceProvider = dispatcherOptions.Services.BuildServiceProvider();
        await serviceProvider.GetRequiredService<CustomDbContext>().Database.EnsureCreatedAsync();
        var logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        var list = await logService.RetrieveEventLogsFailedToPublishAsync(100, 10, 60);
        Assert.IsTrue(!list.Any());
    }

    [TestMethod]
    public async Task TestRetrieveEventLogsFailedToPublish2Async()
    {
        var response = await InitializeAsync();

        var logs = await response.CustomDbContext.Set<IntegrationEventLog>().ToListAsync();
        response.CustomDbContext.Set<IntegrationEventLog>().RemoveRange(logs);

        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        await logService.SaveEventAsync(@event, (await response.CustomDbContext.Database.BeginTransactionAsync()).GetDbTransaction());
        await response.CustomDbContext.Database.CommitTransactionAsync();

        var list = (await logService.RetrieveEventLogsPendingToPublishAsync(100)).ToList();
        Assert.IsTrue(list.Count == 1);

        var eventLog = list.Select(log => log.Event).FirstOrDefault()!;
        Assert.IsTrue(eventLog.Equals(@event));
    }

    [TestMethod]
    public async Task TestSaveEventAsync()
    {
        var response = await InitializeAsync();

        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using (var transaction = await response.CustomDbContext.Database.BeginTransactionAsync())
        {
            var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var @event = new OrderPaymentSucceededIntegrationEvent
            {
                OrderId = "1234567890123",
                PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
            };
            await logService.SaveEventAsync(@event, transaction.GetDbTransaction());
            await transaction.CommitAsync();
        }

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.NotPublished) == 1);
    }

    [TestMethod]
    public async Task TestSaveEventByExceptionAsync()
    {
        var response = await InitializeAsync();

        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
            var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var @event = new OrderPaymentSucceededIntegrationEvent
            {
                OrderId = "1234567890123",
                PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
            };
            await logService.SaveEventAsync(@event, transaction.GetDbTransaction());
            throw new Exception("custom exception");
        }, "custom exception");

        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());
    }

    [TestMethod]
    public async Task TestMarkEventAsInProgressAsync()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());

        await logService.MarkEventAsInProgressAsync(@event.Id, 10);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.InProgress) == 1);
        await transaction.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.InProgress) == 1);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);
    }


    [TestMethod]
    public async Task TestMultiJobMarkEventAsInProgressAsync()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());

        await logService.MarkEventAsInProgressAsync(@event.Id, 10);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.InProgress) == 1);
        await transaction.CommitAsync();

        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async ()
            => await logService.MarkEventAsInProgressAsync(@event.Id, 60, default));
    }

    [TestMethod]
    public async Task TestMarkEventAsInProgress2Async()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();


        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await response.CustomDbContext.Set<IntegrationEventLog>().AddAsync(new IntegrationEventLog(@event, Guid.NewGuid())
        {
            State = IntegrationEventStates.Published
        });

        await response.CustomDbContext.SaveChangesAsync();

        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await logService.MarkEventAsInProgressAsync(@event.Id, 10));
    }

    [TestMethod]
    public async Task TestMarkEventAsPublishedAsync()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());

        await logService.MarkEventAsInProgressAsync(@event.Id, 10);

        await logService.MarkEventAsPublishedAsync(@event.Id);

        await transaction.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.Published) == 1);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);
    }

    [TestMethod]
    public async Task TestMarkEventAsPublished2Async()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());

        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await logService.MarkEventAsPublishedAsync(@event.Id));
    }

    [TestMethod]
    public async Task TestMarkEventAsFailedAsync()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());
        await logService.MarkEventAsInProgressAsync(@event.Id, 10);
        await logService.MarkEventAsFailedAsync(@event.Id);
        await transaction.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.PublishedFailed) == 1);
    }

    [TestMethod]
    public async Task TestMarkEventAsFailed2Async()
    {
        var response = await InitializeAsync();
        Assert.IsFalse(await response.CustomDbContext.Set<IntegrationEventLog>().AnyAsync());

        await using var transaction = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transaction.GetDbTransaction());
        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await logService.MarkEventAsFailedAsync(@event.Id));
    }

    private async Task<(CustomDbContext CustomDbContext, IServiceProvider ServiceProvider)> InitializeAsync()
    {
        var dispatcherOptions = CreateDispatcherOptions(new ServiceCollection());
        dispatcherOptions.UseEventLog<CustomDbContext>();
        dispatcherOptions.Services.AddMasaDbContext<CustomDbContext>(option =>
            option.UseTestSqlite(Connection));
        dispatcherOptions.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        Mock<IIntegrationEventBus> integrationEventBus = new();
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
        integrationEventBus.Setup(eventBus => eventBus.GetAllEventTypes()).Returns(types).Verifiable();
        dispatcherOptions.Services.AddScoped(_ => integrationEventBus.Object);
        var serviceProvider = dispatcherOptions.Services.BuildServiceProvider();
        var customDbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await customDbContext.Database.EnsureCreatedAsync();
        return new(customDbContext, serviceProvider);
    }

    [TestMethod]
    public async Task TestDeleteExpiresAsync()
    {
        var response = await InitializeAsync();

        #region Initialization data

        var logs = await response.CustomDbContext.Set<IntegrationEventLog>().ToListAsync();
        response.CustomDbContext.Set<IntegrationEventLog>().RemoveRange(logs);

        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var integrationEventLogs = new List<IntegrationEventLog>()
        {
            new(@event, Guid.NewGuid())
            {
                State = IntegrationEventStates.InProgress,
                ModificationTime = DateTime.UtcNow.AddSeconds(-120),
            },
            new(@event, Guid.NewGuid())
            {
                State = IntegrationEventStates.Published,
                ModificationTime = DateTime.UtcNow.AddSeconds(-120),
            },
        };
        await response.CustomDbContext.Set<IntegrationEventLog>().AddRangeAsync(integrationEventLogs);
        await response.CustomDbContext.SaveChangesAsync();

        Assert.AreEqual(integrationEventLogs.Count, await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync());

        #endregion

        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();

        await logService.DeleteExpiresAsync(DateTime.UtcNow, 100);
        Assert.AreEqual(integrationEventLogs.Count(e => e.State != IntegrationEventStates.Published),
            await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync());
    }
}
