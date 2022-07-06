// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class FeaturesTest : TestBase
{
    private readonly IEventBus _eventBus;

    public FeaturesTest() : base()
    {
        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [TestMethod]
    public async Task TestMethodsReturnType()
    {
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            try
            {
                ResetMemoryEventBus(typeof(AddBasketEvent).Assembly);
            }
            catch (Exception)
            {
                ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
            await Task.CompletedTask;
        });
    }

    [TestMethod]
    public async Task TestNotImplementationIEvent()
    {
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
        {
            try
            {
                ResetMemoryEventBus(typeof(AddCatalogEvent).Assembly);
            }
            catch (Exception)
            {
                ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
            await Task.CompletedTask;
        });
    }

    [TestMethod]
    public async Task TestMultiParameter()
    {
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
        {
            try
            {
                ResetMemoryEventBus(typeof(AddGoodsEvent).Assembly);
            }
            catch (Exception)
            {
                ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
            await Task.CompletedTask;
        });
    }

    [TestMethod]
    public async Task TestCorrectEventBus()
    {
        AddShoppingCartEvent @event = new AddShoppingCartEvent()
        {
            GoodsId = Guid.NewGuid().ToString(),
            Count = 1
        };
        await _eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestNullEvent()
    {
        AddShoppingCartEvent? @event = null;
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _eventBus.PublishAsync(@event!));
    }

    [DataTestMethod]
    [DataRow("50", 2, "30", "70")]
    [DataRow("60", 5, "140", "160")]
    public async Task TestMultiHandler(string price, int count, string discountAmount, string payAmount)
    {
        ComputeEvent @event = new ComputeEvent()
        {
            Price = Convert.ToDecimal(price),
            Count = count
        };
        await _eventBus.PublishAsync(@event);
        Assert.IsTrue(@event.DiscountAmount == Convert.ToDecimal(discountAmount) && @event.PayAmount == Convert.ToDecimal(payAmount));
    }

    [TestMethod]
    public async Task TestNotParameter()
    {
        await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () =>
        {
            try
            {
                ResetMemoryEventBus(typeof(DeleteGoodsEvent).Assembly);
            }
            catch (Exception)
            {
                ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
            await Task.CompletedTask;
        });
    }

    [TestMethod]
    public Task TestOnlyCancelHandler()
    {
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            try
            {
                ResetMemoryEventBus(typeof(OnlyCancelHandler.Tests.Events.BindPhoneNumberEvent).Assembly);
            }
            catch (NotSupportedException)
            {
                ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
        });
        return Task.CompletedTask;
    }

    [TestMethod]
    public async Task TestHandlerIsIgnore()
    {
        var @event = new OrderPaymentSucceededEvent()
        {
            OrderId = "123456789012",
            Timespan = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
        };
        await _eventBus.PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestPublishIntegrationEvent()
    {
        var @event = new OrderPaymentFailedIntegrationEvent()
        {
            OrderId = "123456789012",
        };
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            await _eventBus.PublishAsync(@event);
        });
    }

    [TestMethod]
    public async Task TestPublishIntegrationEventAndUseUoW()
    {
        base.ResetMemoryEventBus(services =>
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            services.AddScoped(serviceProvider => unitOfWork.Object);
            return services;
        }, true, typeof(AssemblyResolutionTests).Assembly);
        var @event = new OrderPaymentFailedIntegrationEvent()
        {
            OrderId = "123456789012",
        };
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            await _services.BuildServiceProvider().GetRequiredService<IEventBus>().PublishAsync(@event);
        });
    }

    [TestMethod]
    public async Task TestTransferEventAndOpenTransaction()
    {
        base.ResetMemoryEventBus(services =>
        {
            var uoW = new Mock<IUnitOfWork>();
            uoW.Setup(x => x.TransactionHasBegun).Returns(true);
            uoW.Setup(e => e.CommitAsync(CancellationToken.None)).Verifiable();
            services.AddScoped(serviceProvider => uoW.Object);
            return services;
        }, true, typeof(AssemblyResolutionTests).Assembly);
        var @event = new DeductionMoneyEvent()
        {
            Account = "tom",
            PayeeAccount = "Jim",
            Money = 100
        };
        await _services.BuildServiceProvider().GetRequiredService<IEventBus>().PublishAsync(@event);
    }

    [TestMethod]
    public async Task TestCommitAsync()
    {
        base.ResetMemoryEventBus(services =>
        {
            return services;
        }, true, typeof(AssemblyResolutionTests).Assembly);
        var @event = new DeductionMoneyEvent()
        {
            Account = "tom",
            PayeeAccount = "Jim",
            Money = 100
        };
        var serviceProvider = _services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();

        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await eventBus.CommitAsync(default));
    }

    [TestMethod]
    public async Task TestUseUoWCommitAsync()
    {
        var uoW = new Mock<IUnitOfWork>();
        base.ResetMemoryEventBus(services =>
        {
            uoW.Setup(e => e.CommitAsync(CancellationToken.None)).Verifiable();
            services.AddScoped(serviceProvider => uoW.Object);
            return services;
        }, true, typeof(AssemblyResolutionTests).Assembly);
        var @event = new DeductionMoneyEvent()
        {
            Account = "tom",
            PayeeAccount = "Jim",
            Money = 100
        };
        var serviceProvider = _services.BuildServiceProvider();
        var eventBus = serviceProvider.GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(@event);

        uoW.Verify(u => u.CommitAsync(default), Times.Once);
    }

    [DataTestMethod]
    [DataRow(1, 2, -1)]
    [DataRow(5, 4, 24)]
    public async Task TestEventBusCancelOrder(int parameterA, int parameterB, int result)
    {
        IServiceCollection services = new ServiceCollection();
        services.AddTestEventBus(new[] { typeof(CalculateEvent).Assembly }, ServiceLifetime.Scoped);
        var @event = new CalculateEvent()
        {
            ParameterA = parameterA,
            ParameterB = parameterB
        };
        var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
        await eventBus.PublishAsync(@event);
        Assert.IsTrue(@event.Result == result);
    }

    [DataTestMethod]
    public void TestEventHandler()
    {
        var order = 1;
        bool enableRetry = true;
        var failureLevels = FailureLevels.ThrowAndCancel;
        bool isCancel = true;
        int retryTimes = 5;
        int defaultRetryTimes = 3;

        var eventAttribute = new EventHandlerAttribute(order);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == false &&
            eventAttribute.RetryTimes == 0 &&
            eventAttribute.FailureLevels == FailureLevels.Throw &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, failureLevels);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == false &&
            eventAttribute.RetryTimes == 0 &&
            eventAttribute.FailureLevels == failureLevels &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, failureLevels, enableRetry);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == enableRetry &&
            eventAttribute.RetryTimes == defaultRetryTimes &&
            eventAttribute.FailureLevels == failureLevels &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, failureLevels, enableRetry, isCancel);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == enableRetry &&
            eventAttribute.RetryTimes == defaultRetryTimes &&
            eventAttribute.FailureLevels == failureLevels &&
            eventAttribute.IsCancel == isCancel
        );

        eventAttribute = new EventHandlerAttribute(order, failureLevels, enableRetry, retryTimes);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == enableRetry &&
            eventAttribute.RetryTimes == retryTimes &&
            eventAttribute.FailureLevels == failureLevels &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, failureLevels, enableRetry, retryTimes, isCancel);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry == enableRetry &&
            eventAttribute.RetryTimes == retryTimes &&
            eventAttribute.FailureLevels == failureLevels &&
            eventAttribute.IsCancel == isCancel
        );

        eventAttribute = new EventHandlerAttribute(order, enableRetry);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry &&
            eventAttribute.RetryTimes == defaultRetryTimes &&
            eventAttribute.FailureLevels == FailureLevels.Throw &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, enableRetry, retryTimes);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry &&
            eventAttribute.RetryTimes == retryTimes &&
            eventAttribute.FailureLevels == FailureLevels.Throw &&
            eventAttribute.IsCancel == false
        );

        eventAttribute = new EventHandlerAttribute(order, enableRetry, isCancel);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry &&
            eventAttribute.RetryTimes == defaultRetryTimes &&
            eventAttribute.FailureLevels == FailureLevels.Throw &&
            eventAttribute.IsCancel == isCancel
        );

        eventAttribute = new EventHandlerAttribute(order, enableRetry, isCancel, retryTimes);
        Assert.IsTrue(
            eventAttribute.Order == order &&
            eventAttribute.EnableRetry &&
            eventAttribute.RetryTimes == retryTimes &&
            eventAttribute.FailureLevels == FailureLevels.Throw &&
            eventAttribute.IsCancel == isCancel
        );

    }

    [TestMethod]
    public void TestOrderLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            new EventHandlerAttribute(-10);
        }, "The order must be greater than or equal to 0");
    }

    [TestMethod]
    public async Task TestEventBusExceptionAsync()
    {
        var services = new ServiceCollection();
        services.AddTestEventBus(new Assembly[] { typeof(FeaturesTest).Assembly }, ServiceLifetime.Scoped);
        var registerUserEvent = new RegisterUserEvent("Jim");
        var eventBus = services.BuildServiceProvider().GetRequiredService<IEventBus>();
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () => await eventBus.PublishAsync(registerUserEvent));
    }
}
