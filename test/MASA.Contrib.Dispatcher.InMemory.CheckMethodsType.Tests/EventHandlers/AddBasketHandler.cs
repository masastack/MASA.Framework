namespace MASA.Contrib.Dispatcher.InMemory.CheckMethodsType.Tests.EventHandlers;

public class AddBasketHandler
{
    private readonly ILogger<AddBasketHandler> _logger;
    public AddBasketHandler(ILogger<AddBasketHandler> logger) => _logger = logger;

    [EventHandler]
    public Task<string> AddLog(AddBasketEvent @event)
    {
        _logger.LogInformation($"add basket log：GoogdsId：{@event.GoodsId}，count：{@event.Count}");
        return Task.FromResult("success");
    }
}