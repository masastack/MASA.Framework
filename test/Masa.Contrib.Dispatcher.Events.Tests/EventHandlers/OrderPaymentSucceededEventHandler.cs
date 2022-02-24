namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class OrderPaymentSucceededEventHandler
{
    private readonly ILogger<OrderPaymentSucceededEventHandler> _logger;
    public OrderPaymentSucceededEventHandler(ILogger<OrderPaymentSucceededEventHandler> logger) => _logger = logger;

    [EventHandler(10, FailureLevels.Ignore)]
    public void AddTradeRecords(OrderPaymentSucceededEvent @event)
    {
        _logger.LogInformation("Order paid successfully, add transaction record");
        if (@event.OrderId.Length > 10)
        {
            throw new NotSupportedException("Wrong order number");
        }
    }

    [EventHandler(10, FailureLevels = FailureLevels.Ignore, IsCancel = true)]
    public void Cancel(OrderPaymentSucceededEvent @event)
    {
        _logger.LogInformation("Order paid successfully, rollback transaction record");
    }
}
