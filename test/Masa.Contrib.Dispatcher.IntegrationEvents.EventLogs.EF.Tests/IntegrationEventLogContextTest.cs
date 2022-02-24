namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

[TestClass]
public class IntegrationEventLogContextTest : TestBase
{
    [TestMethod]
    public void TestCreateDbContext()
    {
        var serviceProvider = CreateDefaultProvider();
        var dbContext = serviceProvider.GetRequiredService<IntegrationEventLogContext>();
        var entity = dbContext.Model.GetEntityTypes().FirstOrDefault(entityType => entityType.Name == typeof(IntegrationEventLog).FullName)!;

        Assert.IsTrue(entity.GetTableName() == "IntegrationEventLog");
        var properties = entity.GetProperties().ToList();
        Assert.IsTrue(properties.Where(x => x.Name == "Id").Select(x => x.IsPrimaryKey()).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "Id").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "Content").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "CreationTime").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "State").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "TimesSent").Select(x => x.IsNullable).FirstOrDefault());
        Assert.IsFalse(properties.Where(x => x.Name == "EventTypeName").Select(x => x.IsNullable).FirstOrDefault());
    }
}
