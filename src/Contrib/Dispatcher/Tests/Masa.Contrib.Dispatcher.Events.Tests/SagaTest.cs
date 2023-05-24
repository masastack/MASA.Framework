// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class SagaTest : TestBase
{
    private readonly IEventBus _eventBus;

    public SagaTest()
    {
        _eventBus = ServiceProvider.GetRequiredService<IEventBus>();
    }

    [DataTestMethod]
    [DataRow("60040012", "success", "the delivery and notice success", false)]
    [DataRow("601454112", "error", "the delivery failed, rolling back success", true)]
    public async Task TestExecuteAbnormalExit(string orderId, string orderState, string result, bool isException)
    {
        var @event = new ShipOrderEvent()
        {
            OrderId = orderId,
            OrderState = orderState
        };

        if (isException)
        {
            await Assert.ThrowsExceptionAsync<Exception>(async () => await _eventBus.PublishAsync(@event));
        }
        else
        {
            await _eventBus.PublishAsync(@event);
        }
        Assert.IsTrue(@event.Message == result);
    }

    [TestMethod]
    public async Task TestAddGoodsEventBySaga()
    {
        IEvent @event = new AddGoodsEvent()
        {
            Name = "Microsoft"
        };
        await _eventBus.PublishAsync(@event);
        Assert.IsTrue(((AddGoodsEvent)@event).Stock == 1);
    }

    [DataTestMethod]
    [DataRow("roller", "change password notice", 0)]
    [DataRow("mark", "change password notice @", 1)]
    [DataRow("roller", "change password notice @", 1)]
    [DataRow("jordan", "change password notice @", 1)]
    public async Task TestLastCancelError(string account, string content, int isError)
    {
        ResetMemoryEventBus(false, null!);
        var @event = new ChangePasswordEvent()
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

    [DataTestMethod]
    [DataRow("smith", "alice", "1000", 0)]
    [DataRow("roller", "alice", "1000", 1)]
    [DataRow("eddie", "clark", "2000", 1)]
    [DataRow("thomas", "clark", "20000000", 1)]
    public async Task TestMultiHandlerBySaga(string account, string optAccount, string price, int isError)
    {
        var @event = new TransferEvent()
        {
            Account = account,
            OptAccount = optAccount,
            Price = Convert.ToDecimal(price)
        };
        if (isError == 1)
        {
            await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await _eventBus.PublishAsync(@event));
        }
        else
        {
            await _eventBus.PublishAsync(@event);
        }
    }

    [TestMethod]
    public async Task TestMultiOrderBySaga()
    {
        IEventBus? eventBus = null;
        Assert.ThrowsException<ArgumentException>(() =>
        {
            ResetMemoryEventBus(false, typeof(SagaTest).Assembly, typeof(EditCategoryEvent).Assembly);
            eventBus = ServiceProvider.GetRequiredService<IEventBus>();
        });
        var @event = new EditCategoryEvent()
        {
            CategoryId = new Random().Next(100, 10000).ToString(),
            CategoryName = "Name"
        };
        if (eventBus != null)
        {
            await eventBus.PublishAsync(@event);
        }
        ResetMemoryEventBus(false, null!);
    }


}
