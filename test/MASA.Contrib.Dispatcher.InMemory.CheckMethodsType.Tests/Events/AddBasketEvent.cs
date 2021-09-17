namespace MASA.Contrib.Dispatcher.InMemory.CheckMethodsType.Tests.Events;

public class AddBasketEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}