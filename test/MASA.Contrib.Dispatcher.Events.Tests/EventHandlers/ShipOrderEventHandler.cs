namespace MASA.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class ShipOrderEventHandler : ISagaEventHandler<ShipOrderEvent>
{
    private int ExecCount { get; set; }

    private readonly ILogger<ShipOrderEventHandler> _logger;

    public ShipOrderEventHandler(ILogger<ShipOrderEventHandler> logger)
    {
        _logger = logger;
        ExecCount = 0;
    }

    [EventHandler(10, FailureLevels.ThrowAndCancel, true)]
    public Task HandleAsync(ShipOrderEvent @event)
    {
        ExecCount++;
        if (ExecCount - 1 <= 1)
        {
            throw new Exception("try again");
        }

        _logger.LogInformation("update express information");
        if (@event.OrderId.Length > 8)
        {
            @event.Message = "the delivery failure";
            throw new Exception("the delivery failure");
        }
        @event.Message = "the delivery success";
        return Task.CompletedTask;
    }

    [EventHandler(10, false, true)]
    public Task CancelAsync(ShipOrderEvent @event)
    {
        @event.Message = "the delivery failed, rolling back success";
        _logger.LogInformation("the delivery failed, rolling back success");
        return Task.CompletedTask;
    }
}

public class ShipOrderAndNoticeHandler : IEventHandler<ShipOrderEvent>
{
    private readonly ILogger<ShipOrderAndNoticeHandler> _logger;
    public ShipOrderAndNoticeHandler(ILogger<ShipOrderAndNoticeHandler> logger) => _logger = logger;

    [EventHandler(20)]
    public Task HandleAsync(ShipOrderEvent @event)
    {
        @event.Message = "the delivery and notice success";
        _logger.LogInformation("order delivered successfully");
        return Task.CompletedTask;
    }
}