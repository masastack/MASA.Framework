namespace MASA.Contrib.Dispatcher.Events.OrderLessThanZeroBySaga.Tests.EventHandlers;

public class EditGoodsHandler : IEventHandler<EditGoodsEvent>
{
    private readonly ILogger<EditGoodsHandler>? _logger;
    public EditGoodsHandler(ILogger<EditGoodsHandler>? logger) => _logger = logger;

    [EventHandler(-10)]
    public Task HandleAsync(EditGoodsEvent @event)
    {
        _logger?.LogInformation($"edit goods log，GoodsId:{@event.GoodsId},Name:{@event.GoodsName},CategoryId:{@event.CategoryId}");
        return Task.CompletedTask;
    }
}
