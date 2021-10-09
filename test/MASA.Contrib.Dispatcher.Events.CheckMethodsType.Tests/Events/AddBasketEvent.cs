namespace MASA.Contrib.Dispatcher.Events.CheckMethodsType.Tests.Events;

public class AddBasketEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}
