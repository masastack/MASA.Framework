namespace MASA.Contrib.Dispatcher.IntegrationEvents.Tests;

[TestClass]
public class RegisterServicesBusTest : TestBase
{
    [TestMethod]
    public void TestEmptyDaprPubSubName()
    {
        var daprPubSubName = string.Empty;
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var serviceProvider = CreateCustomerDaprPubSubProvider(daprPubSubName, "topic");
        });
    }
}
