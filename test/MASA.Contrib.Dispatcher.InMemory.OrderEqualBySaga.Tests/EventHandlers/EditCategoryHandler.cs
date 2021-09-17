namespace MASA.Contrib.Dispatcher.InMemory.OrderEqualBySaga.Tests.EventHandlers;

public class EditCategoryHandler : ISagaEventHandler<EditCategoryEvent>
{
    private readonly ILogger<EditCategoryHandler> _logger;
    public EditCategoryHandler(ILogger<EditCategoryHandler> logger) => _logger = logger;

    [EventHandler(10)]
    public Task CancelAsync(EditCategoryEvent @event)
    {
        _logger.LogInformation($"cancel edit category log,CategoryId:{@event.CategoryId},Name:{@event.CategoryName}");
        return Task.CompletedTask;
    }

    [EventHandler(20)]
    public Task HandleAsync(EditCategoryEvent @event)
    {
        _logger.LogInformation($"edit category log,CategoryId:{@event.CategoryId},Name:{@event.CategoryName}");
        return Task.CompletedTask;
    }
}