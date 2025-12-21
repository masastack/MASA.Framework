// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore.Tests;

[TestClass]
public class IntegrationEventLogServiceTest : TestBase
{
    private IServiceCollection _services;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.TryAddEnumerable(new ServiceDescriptor(typeof(IModelCreatingProvider),
            typeof(IntegrationEventLogModelCreatingProvider),
            ServiceLifetime.Singleton));
        _services.TryAddScoped(typeof(IntegrationEventLogContext),
            serviceProvider => new IntegrationEventLogContext(serviceProvider.GetRequiredService<CustomDbContext>()));
        _services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(Connection));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestRetrieveEventLogsPendingToPublishAsync(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => await InsertDataAsync(eventLogContext),
                isUseLogger
            );
        var list = await integrationEventLogService.RetrieveEventLogsPendingToPublishAsync(100, CancellationToken.None);
        Assert.AreEqual(1, list.Count());
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestRetrieveEventLogsFailedToPublishAsync(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => await InsertDataAsync(eventLogContext),
                isUseLogger
            );
        var list = await integrationEventLogService.RetrieveEventLogsFailedToPublishAsync(100, 10, 1, CancellationToken.None);
        Assert.AreEqual(0, list.Count());
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestRetrieveEventLogsFailed2ToPublishAsync(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext =>
                {
                    await InsertDataAsync(eventLogContext);
                    await InsertDataAsync(eventLogContext, IntegrationEventStates.Published);
                    await InsertDataAsync(eventLogContext, IntegrationEventStates.InProgress, DateTime.UtcNow.AddSeconds(-1));
                    await InsertDataAsync(eventLogContext, IntegrationEventStates.PublishedFailed, DateTime.UtcNow.AddSeconds(-1));
                },
                isUseLogger
            );
        var list = await integrationEventLogService.RetrieveEventLogsFailedToPublishAsync(100, 10, 1, CancellationToken.None);
        Assert.AreEqual(2, list.Count());
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestSaveEventAsync(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(_ => Task.CompletedTask,
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();

        var count = integrationEventLogContext.DbContext.Set<IntegrationEventLog>().Count();
        Assert.AreEqual(0, count);

        var orderPaymentSucceededIntegrationEvent = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await using var transaction = await integrationEventLogContext.DbContext.Database.BeginTransactionAsync();
        await integrationEventLogService.SaveEventAsync(orderPaymentSucceededIntegrationEvent, null, transaction.GetDbTransaction());
        await integrationEventLogContext.DbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        count = integrationEventLogContext.DbContext.Set<IntegrationEventLog>().Count();
        Assert.AreEqual(1, count);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsPublishedAsync(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext, IntegrationEventStates.InProgress),
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogService.MarkEventAsPublishedAsync(eventId, CancellationToken.None);
        var count = await integrationEventLogContext.DbContext.Set<IntegrationEventLog>()
            .CountAsync(eventLog => eventLog.EventId == eventId);
        Assert.AreEqual(1, count);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsPublished2Async(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext, IntegrationEventStates.NotPublished),
                isUseLogger
            );
        await Assert.ThrowsExactlyAsync<UserFriendlyException>(async ()
            => await integrationEventLogService.MarkEventAsPublishedAsync(eventId, CancellationToken.None));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsInProgressAsync(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext),
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogService.MarkEventAsInProgressAsync(eventId, 1, CancellationToken.None);
        var count = await integrationEventLogContext.DbContext.Set<IntegrationEventLog>()
            .CountAsync(eventLog => eventLog.EventId == eventId);
        Assert.AreEqual(1, count);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsInProgress2Async(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext, IntegrationEventStates.Published),
                isUseLogger
            );
        await Assert.ThrowsExactlyAsync<UserFriendlyException>(async ()
            => await integrationEventLogService.MarkEventAsInProgressAsync(eventId, 1, CancellationToken.None));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsInProgress3Async(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId =
                    await InsertDataAsync(eventLogContext, IntegrationEventStates.InProgress, DateTime.UtcNow.AddSeconds(1)),
                isUseLogger
            );
        await Assert.ThrowsExactlyAsync<UserFriendlyException>(async ()
            => await integrationEventLogService.MarkEventAsInProgressAsync(eventId, 10, CancellationToken.None));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsFailedAsync(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext, IntegrationEventStates.InProgress),
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogService.MarkEventAsFailedAsync(eventId, CancellationToken.None);
        var count = await integrationEventLogContext.DbContext.Set<IntegrationEventLog>()
            .CountAsync(eventLog => eventLog.EventId == eventId && eventLog.State == IntegrationEventStates.PublishedFailed);
        Assert.AreEqual(1, count);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsFailed2Async(bool isUseLogger)
    {
        Guid eventId = default!;
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => eventId = await InsertDataAsync(eventLogContext),
                isUseLogger
            );
        await Assert.ThrowsExactlyAsync<UserFriendlyException>(async ()
            => await integrationEventLogService.MarkEventAsFailedAsync(eventId, CancellationToken.None));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestMarkEventAsFailed3Async(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => _ = await InsertDataAsync(eventLogContext),
                isUseLogger
            );
        await Assert.ThrowsExactlyAsync<ArgumentException>(async ()
            => await integrationEventLogService.MarkEventAsFailedAsync(Guid.NewGuid(), CancellationToken.None));
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestDeleteExpiresAsync(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => await InsertDataAsync(eventLogContext, IntegrationEventStates.Published),
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogService.DeleteExpiresAsync(DateTime.UtcNow.AddSeconds(1), 1, CancellationToken.None);
        var count = await integrationEventLogContext.DbContext.Set<IntegrationEventLog>()
            .CountAsync();
        Assert.AreEqual(0, count);
    }

    [DataRow(true)]
    [DataRow(false)]
    [DataTestMethod]
    public async Task TestDeleteExpires2Async(bool isUseLogger)
    {
        var integrationEventLogService =
            await CreateIntegrationEventLogServiceAsync(async eventLogContext => await InsertDataAsync(eventLogContext, IntegrationEventStates.NotPublished),
                isUseLogger
            );
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogService.DeleteExpiresAsync(DateTime.UtcNow.AddSeconds(-1), 1, CancellationToken.None);
        var count = await integrationEventLogContext.DbContext.Set<IntegrationEventLog>()
            .CountAsync();
        Assert.AreEqual(1, count);
    }

    private async Task<IntegrationEventLogService> CreateIntegrationEventLogServiceAsync(
        Func<IntegrationEventLogContext, Task> func,
        bool isUseLogger)
    {
        if (isUseLogger) _services.AddLogging();
        var serviceProvider = _services.BuildServiceProvider();
        var integrationEventLogContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        await integrationEventLogContext.DbContext.Database.EnsureCreatedAsync();
        await func.Invoke(integrationEventLogContext);
        var logger = isUseLogger ? serviceProvider.GetRequiredService<ILogger<IntegrationEventLogService>>() : null;
        return new IntegrationEventLogService(integrationEventLogContext, logger);
    }

    private static async Task<Guid> InsertDataAsync(
        IntegrationEventLogContext integrationEventLogContext,
        IntegrationEventStates? integrationEventStates = null,
        DateTime? modificationTime = null)
    {
        var orderPaymentSucceededIntegrationEvent = new OrderPaymentSucceededIntegrationEvent
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await using var transaction = await integrationEventLogContext.DbContext.Database.BeginTransactionAsync();
        var integrationEventLog = new IntegrationEventLog(orderPaymentSucceededIntegrationEvent, transaction.TransactionId)
        {
            State = integrationEventStates ?? IntegrationEventStates.NotPublished
        };
        if (modificationTime != null)
            integrationEventLog.ModificationTime = modificationTime.Value;
        await integrationEventLogContext.DbContext.Set<IntegrationEventLog>().AddAsync(integrationEventLog);
        await integrationEventLogContext.DbContext.SaveChangesAsync();
        await integrationEventLogContext.DbContext.Database.CommitTransactionAsync();
        integrationEventLogContext.DbContext.Entry(integrationEventLog).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        return integrationEventLog.EventId;
    }
}
