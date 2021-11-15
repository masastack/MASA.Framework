namespace MASA.Contrib.Dispatcher.Events.Tests;

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
        AddShoppingCartEvent @event = null;
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await _eventBus.PublishAsync(@event));
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
        var @event = new DeleteGoodsEvent()
        {
            CreationTime = DateTime.Now,
        };
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
                ResetMemoryEventBus(typeof(OrderStockConfirmedEvent).Assembly);
            }
            catch (ArgumentOutOfRangeException)
            {
                try
                {
                    ResetMemoryEventBus(typeof(FeaturesTest).Assembly);
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
                ResetMemoryEventBus(typeof(OnlyCancelHandler.Tests.Events.BindMobileEvent).Assembly);
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

        await eventBus.CommitAsync(default);
        uoW.Verify(u => u.CommitAsync(default), Times.Once);
    }
}
