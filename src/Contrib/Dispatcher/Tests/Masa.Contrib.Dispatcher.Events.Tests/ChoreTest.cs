// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class ChoreTest : TestBase
{
    private readonly IEventBus _eventBus;
    public ChoreTest()
    {
        _eventBus = ServiceProvider.GetRequiredService<IEventBus>();
    }

    [DataTestMethod]
    [DataRow("jordan", "19999999999", 1, "A very ordinary boy who likes code")]
    [DataRow("tom", "18888888888", 0, "A girl who likes to dance")]
    public async Task TestNotHandler(string account, string phone, int gender, string abstracts)
    {
        var @event = new AddUserEvent()
        {
            Account = account,
            Phone = phone,
            Gender = gender == 1,
            Abstract = abstracts
        };
        await Assert.ThrowsExceptionAsync<UserFriendlyException>(async () =>
        {
            await _eventBus.PublishAsync(@event);
        });
    }

    [TestMethod]
    public void TestDispatchHandlerConstructor()
    {
        var dispatchHandler = new EventHandlerAttribute();
        Assert.IsTrue(dispatchHandler.Order.Equals(int.MaxValue));

        dispatchHandler = new EventHandlerAttribute(1);
        Assert.IsTrue(dispatchHandler.Order.Equals(1));

        dispatchHandler = new EventHandlerAttribute(1, true);
        Assert.IsTrue(dispatchHandler.Order.Equals(1));
        Assert.IsTrue(dispatchHandler.EnableRetry.Equals(true));

        dispatchHandler = new EventHandlerAttribute(2, FailureLevels.Ignore);
        Assert.IsTrue(dispatchHandler.Order.Equals(2));
        Assert.IsTrue(dispatchHandler.EnableRetry.Equals(false));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.Ignore));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(0));

        dispatchHandler = new EventHandlerAttribute(10, true, false);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.EnableRetry.Equals(true));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(3));

        dispatchHandler = new EventHandlerAttribute(10, true, false, 5);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.EnableRetry.Equals(true));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(5));

        dispatchHandler = new EventHandlerAttribute(10, false, false, 5);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.EnableRetry.Equals(false));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(0));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.ThrowAndCancel, true);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.ThrowAndCancel));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(3));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.Ignore, true, false);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.Ignore));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(3));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.Ignore, false, true);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.Ignore));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(0));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(true));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.ThrowAndCancel, true, 10);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.ThrowAndCancel));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(10));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.ThrowAndCancel, false, 10);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.ThrowAndCancel));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(0));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(false));

        dispatchHandler = new EventHandlerAttribute(10, FailureLevels.ThrowAndCancel, true, 5, true);
        Assert.IsTrue(dispatchHandler.Order.Equals(10));
        Assert.IsTrue(dispatchHandler.FailureLevels.Equals(FailureLevels.ThrowAndCancel));
        Assert.IsTrue(dispatchHandler.RetryTimes.Equals(5));
        Assert.IsTrue(dispatchHandler.IsCancel.Equals(true));
    }
}
