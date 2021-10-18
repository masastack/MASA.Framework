namespace MASA.Contrib.Dispatcher.Events.CheckMethodsParameterNotNull.Tests.Events;

public record DeleteGoodsEvent : Event
{
    public string GoodsId { get; set; }
}
