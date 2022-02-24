namespace Masa.Contrib.Dispatcher.Events.CheckMethodsType.Tests.Events;

public record AddBasketEvent : Event
{
    public string GoodsId { get; set; }

    public int Count { get; set; }
}
