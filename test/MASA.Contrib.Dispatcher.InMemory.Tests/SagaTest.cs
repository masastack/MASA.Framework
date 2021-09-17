namespace MASA.Contrib.Dispatcher.InMemory.Tests;

[TestClass]
public class SagaTest : TestBase
{
    private readonly IEventBus _eventBus;
    public SagaTest() : base()
    {
        _eventBus = _serviceProvider.GetService<IEventBus>();
    }

    [DataTestMethod]
    [DataRow("60040012", "success", "the delivery and notice success")]
    [DataRow("601454112", "error", "the delivery failed, rolling back success")]
    public async Task TestExecuteAbnormalExit(string orderId, string orderState, string result)
    {
        ShipOrderEvent @event = new ShipOrderEvent()
        {
            OrderId = orderId,
            OrderState = orderState
        };
        await _eventBus.PublishAsync(@event);
        Assert.IsTrue(@event.Message == result);
    }

    [DataTestMethod]
    [DataRow("roller", "change password notcices", 0)]
    [DataRow("mark", "change password notcices @", 1)]
    [DataRow("roller", "change password notcices @", 0)]
    [DataRow("jordan", "change password notcices @", 0)]
    public async Task TestLastCancelError(string account, string content, int isError)
    {
        base.ResetMemoryEventBus(false, null);
        ChangePasswordEvent @event = new ChangePasswordEvent()
        {
            Account = account,
            Content = content
        };
        if (isError == 1)
        {
            await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            {
                await _eventBus.PublishAsync(@event);
            });
        }
        else
        {
            await _eventBus.PublishAsync(@event);
        }
    }

    [TestMethod]
    public async Task TestEqualMethodsBySaga()
    {
        EditUserEvent @event = new EditUserEvent()
        {
            UserId = new Random().Next(10, 1000000).ToString(),
            UserName = "roller"
        };
        await _eventBus.PublishAsync(@event);
    }

    [DataTestMethod]
    [DataRow("smith", "alice", "1000", 0)]
    [DataRow("roller", "alice", "1000", 1)]
    [DataRow("eddie", "clark", "2000", 0)]
    [DataRow("eddie", "clark", "20000000", 1)]
    public async Task TestMultiHandlerBySaga(string account, string optAccount, string price, int isError)
    {
        TransferEvent @event = new TransferEvent()
        {
            Account = account,
            OptAccount = optAccount,
            Price = Convert.ToDecimal(price)
        };
        if (isError == 1)
        {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
            {
                await _eventBus.PublishAsync(@event);
            });
        }
        else
        {
            await _eventBus.PublishAsync(@event);
        }
    }

    [TestMethod]
    public async Task TestMultiOrderBySaga()
    {
        IEventBus eventBus = null;
        Assert.ThrowsException<ArgumentException>(() =>
        {
            base.ResetMemoryEventBus(false, typeof(SagaTest).Assembly, typeof(EditCategoryEvent).Assembly);
            eventBus = base._serviceProvider.GetRequiredService<IEventBus>();
        });
        EditCategoryEvent @event = new EditCategoryEvent()
        {
            CategoryId = new Random().Next(100, 10000).ToString(),
            CategoryName = "Name"
        };
        if (eventBus != null)
        {
            await eventBus.PublishAsync(@event);
        }
        base.ResetMemoryEventBus(false, null);
    }

    [TestMethod]
    public async Task TestLessThenZeroBySaga()
    {
        IEventBus eventBus = null;
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            base.ResetMemoryEventBus(false, typeof(EditGoodsEvent).Assembly);
            eventBus = base._serviceProvider.GetRequiredService<IEventBus>();
        });
        EditGoodsEvent @event = new EditGoodsEvent()
        {
            GoodsId = new Random().Next(10, 1000).ToString(),
            CategoryId = new Random().Next(100, 10000).ToString(),
            GoodsName = "Name"
        };
        if (eventBus != null)
        {
            await eventBus.PublishAsync(@event);
        }
        base.ResetMemoryEventBus(false, null);
    }
}