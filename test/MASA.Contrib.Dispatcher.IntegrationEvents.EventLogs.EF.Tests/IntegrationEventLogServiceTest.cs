namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

[TestClass]
public class IntegrationEventLogServiceTest : TestBase
{
    [TestMethod]
    public void TestNullDbTransaction()
    {
        DbTransaction transaction = null!;
        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        var serviceProvider = CreateDefaultProvider();
        var dbContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        Assert.ThrowsException<ArgumentNullException>(() => eventLogService.SaveEventAsync(@event, transaction));
    }

    [TestMethod]
    public async Task TestEventLogService()
    {
        var serviceProvider = CreateDefaultProvider();
        var dbContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        dbContext.Database.EnsureCreated();
        var transaction = dbContext.Database.GetDbConnection().BeginTransaction();
        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        await eventLogService.SaveEventAsync(@event, transaction);

        var transactionId = dbContext.Database.CurrentTransaction!.TransactionId;

        var eventLog = dbContext.EventLogs.FirstOrDefault();
        Assert.IsNotNull(eventLog);
        Assert.IsTrue(eventLog.State == IntegrationEventStates.NotPublished);
        Assert.IsTrue(eventLog.Id == @event.Id);

        var eventLogs = await eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);
        Assert.IsNotNull(eventLogs.Count() == 1);
        eventLog = dbContext.EventLogs.FirstOrDefault();
        Assert.IsNotNull(eventLog);
        Assert.IsTrue(eventLog.State == IntegrationEventStates.NotPublished);
        Assert.IsTrue(eventLog.Id == @event.Id);


        await eventLogService.MarkEventAsInProgressAsync(eventLog.Id);
        eventLog = dbContext.EventLogs.Where(x => x.Id == eventLog.Id).FirstOrDefault();
        Assert.IsNotNull(eventLog);
        Assert.IsTrue(eventLog.State == IntegrationEventStates.InProgress);
        Assert.IsTrue(eventLog.TimesSent == 1);

        await eventLogService.MarkEventAsPublishedAsync(eventLog.Id);
        eventLog = dbContext.EventLogs.Where(x => x.Id == eventLog.Id).FirstOrDefault();
        Assert.IsNotNull(eventLog);
        Assert.IsTrue(eventLog.State == IntegrationEventStates.Published);

        await eventLogService.MarkEventAsFailedAsync(eventLog.Id);
        eventLog = dbContext.EventLogs.Where(x => x.Id == eventLog.Id).FirstOrDefault();
        Assert.IsNotNull(eventLog);
        Assert.IsTrue(eventLog.State == IntegrationEventStates.PublishedFailed);

        eventLogs = await eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);
        Assert.IsNotNull(eventLogs.Count() == 0);
    }
}
