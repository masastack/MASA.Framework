namespace MASA.Contrib.Dispatcher.Events.CheckMethodsParameter.Tests.Events;

public record AddGoodsEvent : Event
{
    public string GoodsId { get; set; }

    public string CategoryId { get; set; }

    public string GoodsName { get; set; }
}
