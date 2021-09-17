namespace MASA.Contrib.Dispatcher.InMemory.OrderLessThanZeroByFeature.Tests.EventHandlers;

public class OrderStockConfirmedHandler
{
    private readonly ILogger<OrderStockConfirmedHandler> _logger;

    public OrderStockConfirmedHandler(ILogger<OrderStockConfirmedHandler> logger) => _logger = logger;

    [EventHandler(-10)]
    public void AddLog(OrderStockConfirmedEvent @event)
    {
        _logger.LogInformation($"add order stock confirmed log,orderId:{@event.OrderId}");
    }
}
