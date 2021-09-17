namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class AddShoppingCartEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}