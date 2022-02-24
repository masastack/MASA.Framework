namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

[TestClass]
public class IntegrationEventLogServiceTest : TestBase
{
    [TestMethod]
    public async Task TestNullDbTransactionAsync()
    {
        DbTransaction transaction = null!;
        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        var serviceProvider = CreateDefaultProvider();
        var dbContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await eventLogService.SaveEventAsync(@event, transaction));
    }

    [TestMethod]
    public void TestMultUseEventLogService()
    {
        var serviceProvider = CreateDefaultProvider(options =>
        {
            options.UseEventLog(dbContextOptionsBuilder => dbContextOptionsBuilder.UseSqlite(_connection));
        });
        Assert.IsTrue(serviceProvider.GetServices<IntegrationEventLogContext>().Count() == 1);
    }

    [TestMethod]
    public void TestNullServices()
    {
        var options = new DispatcherOptions(null!);
        Assert.ThrowsException<ArgumentNullException>(() => { options.UseEventLog(options => { options.UseSqlite(base._connection); }); });
    }

    [TestMethod]
    public void TestNullDbContextOptionsBuilder()
    {
        var options = new DispatcherOptions(new ServiceCollection());
        Assert.ThrowsException<ArgumentNullException>(() => { options.UseEventLog(null!); });
    }

    [TestMethod]
    public void TestUseCustomDbContextByNullServices()
    {
        var options = new DispatcherOptions(null!);
        Assert.IsNull(options.Services);
        Assert.ThrowsException<ArgumentNullException>(() => options.UseEventLog<CustomDbContext>());
    }

    [TestMethod]
    public void TestGenericEventLog()
    {
        var options = new DispatcherOptions(new ServiceCollection());
        Assert.ThrowsException<NotSupportedException>(() => options.UseEventLog<IntegrationEventLogContext>());
    }

    [TestMethod]
    public async Task TestCustomDbContextAsync()
    {
        var options = new DispatcherOptions(new ServiceCollection());
        options.Services.AddMasaDbContext<CustomDbContext>(options =>
            options.UseSqlite(_connection).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        integrationEventBus.Setup(e => e.GetAllEventTypes()).Returns(() =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type)));
        options.Services.AddScoped(serviceProvider => integrationEventBus.Object);

        options.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

        options.UseEventLog<CustomDbContext>();

        var serviceProvider = options.Services.BuildServiceProvider();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        // using (var transaction = await dbContext.Database.BeginTransactionAsync())
        // {
        //     await eventLogService.SaveEventAsync(@event,
        //         Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(transaction));
        //
        //     await eventLogService.RetrieveEventLogsPendingToPublishAsync(transaction.TransactionId);
        // }
    }

    [TestMethod]
    public async Task TestAddMultEventLog()
    {
        var options = new DispatcherOptions(new ServiceCollection());
        options.Services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(_connection));

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        integrationEventBus.Setup(e => e.GetAllEventTypes()).Returns(() =>
            AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type)));
        options.Services.AddScoped(serviceProvider => integrationEventBus.Object);

        options.Services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();

        options.UseEventLog<CustomDbContext>().UseEventLog<CustomDbContext>();

        var serviceProvider = options.Services.BuildServiceProvider();
        var eventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();

        var @event = new OrderPaymentSucceededIntegrationEvent()
        {
            OrderId = "1234567890123",
            PaymentTime = (long) (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };

        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        await dbContext.Database.EnsureCreatedAsync();
        // using (var transaction = dbContext.Database.BeginTransaction())
        // {
        //     await eventLogService.SaveEventAsync(@event,
        //         Microsoft.EntityFrameworkCore.Storage.DbContextTransactionExtensions.GetDbTransaction(transaction));
        //
        //     await eventLogService.RetrieveEventLogsPendingToPublishAsync(transaction.TransactionId);
        // }
    }

    [TestMethod]
    public void TestGetIntegrationEventLogService()
    {
        var services = new ServiceCollection();
        services.AddDbContext<IntegrationEventLogContext>(options => options.UseSqlite(_connection));
        var serviceProvider = services.BuildServiceProvider();
        Assert.ThrowsException<InvalidOperationException>(() =>
        {
            var dbContext = serviceProvider.GetServices<IntegrationEventLogContext>();
        });
    }
}
