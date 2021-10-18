namespace MASA.Contrib.Dispatcher.Events.Tests.Events;

public record AddShoppingCartEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}
