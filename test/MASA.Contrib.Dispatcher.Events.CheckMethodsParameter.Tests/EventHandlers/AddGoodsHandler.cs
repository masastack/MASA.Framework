using MASA.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests.Events;

namespace MASA.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests.EventHandlers;

public class AddGoodsHandler
{
    [EventHandler]
    public void AddGoods(AddGoodsEvent @event, ILogger<AddGoodsHandler> logger)
    {
        logger.LogInformation($"add goods log,GoodsId:{@event.GoodsId},GoodsName:{@event.GoodsName},CategoryId:{@event.CategoryId}");
    }
}
