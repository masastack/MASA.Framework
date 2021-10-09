using MASA.Contrib.Dispatcher.Events.CheckMethodsType.Tests.Events;

namespace MASA.Contrib.Dispatcher.InMemory.Tests;

[TestClass]
public class FeaturesTest : TestBase
{
    private readonly IEventBus _eventBus;
    public FeaturesTest() : base()
    {
        _eventBus = _serviceProvider.GetService<IEventBus>();
    }

    [TestMethod]
    public async Task TestMethodsReturnType()
    {
        await Assert.ThrowsExceptionAsync<NotSupportedException>(async () =>
        {
            try
            {
                base.ResetMemoryEventBus(typeof(AddBasketEvent).Assembly);
            }
            catch (Exception)
            {
                base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
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
                base.ResetMemoryEventBus(typeof(AddCatalogEvent).Assembly);
            }
            catch (Exception)
            {
                base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
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
                base.ResetMemoryEventBus(typeof(AddGoodsEvent).Assembly);
            }
            catch (Exception)
            {
                base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
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
                base.ResetMemoryEventBus(typeof(DeleteGoodsEvent).Assembly);
            }
            catch (Exception)
            {
                base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                throw;
            }
            await Task.CompletedTask;
        });
    }

    [TestMethod]
    public async Task TestThrowException()
    {
        ForgotPasswordEvent @event = new ForgotPasswordEvent()
        {
            Account = new Random().Next(1000, 9000).ToString(),
            Email = new Random().Next(100000, 9000000).ToString() + "@qq.com",
        };
        await Assert.ThrowsExceptionAsync<Exception>(async () =>
        {
            await _eventBus.PublishAsync(@event);
        });
    }

    [TestMethod]
    public Task TestOrderLessThenZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
        {
            try
            {
                base.ResetMemoryEventBus(typeof(OrderStockConfirmedEvent).Assembly);
            }
            catch (ArgumentOutOfRangeException)
            {
                try
                {
                    base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
                }
                catch (Exception ex)
                {

                }
                throw;
            }
        });
        return Task.CompletedTask;
    }

    [TestMethod]
    public Task TestOnlyCancelHandler()
    {
        Assert.ThrowsException<NotSupportedException>(() =>
        {
            try
            {
                base.ResetMemoryEventBus(typeof(OnlyCancelHandler.Tests.Events.BindMobileEvent).Assembly);
            }
            catch (NotSupportedException)
            {
                base.ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
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
}