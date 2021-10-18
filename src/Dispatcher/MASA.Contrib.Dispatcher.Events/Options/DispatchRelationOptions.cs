namespace MASA.Contrib.Dispatcher.Events.Options;

public class DispatchRelationOptions
{
    public EventHandlerAttribute Handler { get; set; } = new();

    public IEnumerable<EventHandlerAttribute> CancelHandlers { get; set; } = new List<EventHandlerAttribute>();

    public DispatchRelationOptions() { }

    public DispatchRelationOptions(EventHandlerAttribute handler) : this() => Handler = handler;

    public void AddCancelHandler(IEnumerable<EventHandlerAttribute> cancelHandlers)
        => CancelHandlers = cancelHandlers;

    public bool IsCancelHandler(EventHandlerAttribute cancelHandler)
    {
        return Handler.FailureLevels == FailureLevels.ThrowAndCancel && cancelHandler.Order <= Handler.Order
        || Handler.FailureLevels == FailureLevels.Throw && cancelHandler.Order < Handler.Order;
    }
}
