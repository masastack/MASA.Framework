// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore.Tests;

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
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await eventLogService.SaveEventAsync(@event, transaction));
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
    public async Task TestRetrieveEventLogsFailedToPublishAsync()
    {
        var dispatcherOptions = CreateDispatcherOptions(new ServiceCollection());
        dispatcherOptions.UseEventLog<CustomDbContext>();
        dispatcherOptions.Services.AddMasaDbContext<CustomDbContext>(option => option.UseTestSqlite(Connection));
        dispatcherOptions.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        var serviceProvider = dispatcherOptions.Services.BuildServiceProvider();
        await serviceProvider.GetRequiredService<CustomDbContext>().Database.EnsureCreatedAsync();
        var logService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        var list = await logService.RetrieveEventLogsFailedToPublishAsync();
        Assert.IsTrue(!list.Any());
    }

    [TestMethod]
    public async Task TestRetrieveEventLogsFailedToPublish2Async()
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
        var list = (await logService.RetrieveEventLogsFailedToPublishAsync()).ToList();
        Assert.IsTrue(list.Count == 1);

        var eventLog = list.Select(log => log.Event).FirstOrDefault()!;
        Assert.IsTrue(eventLog.Equals(@event));
    }

    [TestMethod]
    public async Task TestSaveEventAsync()
    {
        var response = await InitializeAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using (var transcation = await response.CustomDbContext.Database.BeginTransactionAsync())
        {
            var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var @event = new OrderPaymentSucceededIntegrationEvent
            {
                OrderId = "1234567890123",
                PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
            };
            await logService.SaveEventAsync(@event, transcation.GetDbTransaction());
            await transcation.CommitAsync();
        }

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.NotPublished) == 1);
    }

    [TestMethod]
    public async Task TestSaveEventByExceptionAsync()
    {
        var response = await InitializeAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
            var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
            var @event = new OrderPaymentSucceededIntegrationEvent
            {
                OrderId = "1234567890123",
                PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
            };
            await logService.SaveEventAsync(@event, transcation.GetDbTransaction());
            throw new Exception("custom exception");
        }, "custom exception");

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);
    }

    [TestMethod]
    public async Task TestMarkEventAsInProgressAsync()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transcation.GetDbTransaction());

        await logService.MarkEventAsInProgressAsync(@event.Id);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.InProgress) == 1);
        await transcation.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.InProgress) == 1);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);
    }

    [TestMethod]
    public async Task TestMarkEventAsInProgress2Async()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
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

        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await logService.MarkEventAsInProgressAsync(@event.Id));
    }

    [TestMethod]
    public async Task TestMarkEventAsPublishedAsync()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transcation.GetDbTransaction());

        await logService.MarkEventAsInProgressAsync(@event.Id);

        await logService.MarkEventAsPublishedAsync(@event.Id);

        await transcation.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>()
            .CountAsync(log => log.State == IntegrationEventStates.Published) == 1);
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 1);
    }

    [TestMethod]
    public async Task TestMarkEventAsPublished2Async()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transcation.GetDbTransaction());

        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () => await logService.MarkEventAsPublishedAsync(@event.Id));
    }

    [TestMethod]
    public async Task TestMarkEventAsFailedAsync()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transcation.GetDbTransaction());
        await logService.MarkEventAsInProgressAsync(@event.Id);
        await logService.MarkEventAsFailedAsync(@event.Id);
        await transcation.CommitAsync();

        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync(log => log.State == IntegrationEventStates.PublishedFailed) == 1);
    }

    [TestMethod]
    public async Task TestMarkEventAsFailed2Async()
    {
        var response = await InitializeAsync();
        Assert.IsTrue(await response.CustomDbContext.Set<IntegrationEventLog>().CountAsync() == 0);

        await using var transcation = await response.CustomDbContext.Database.BeginTransactionAsync();
        var logService = response.ServiceProvider.GetRequiredService<IIntegrationEventLogService>();
        var @event = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await logService.SaveEventAsync(@event, transcation.GetDbTransaction());
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
}
