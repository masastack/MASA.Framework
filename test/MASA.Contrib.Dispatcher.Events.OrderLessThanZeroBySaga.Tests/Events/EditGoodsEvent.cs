namespace MASA.Contrib.Dispatcher.Events.OrderLessThanZeroBySaga.Tests.Events;

public record EditGoodsEvent : Event
{
    public string GoodsId { get; set; }

    public string CategoryId { get; set; }

    public string GoodsName { get; set; }
}
