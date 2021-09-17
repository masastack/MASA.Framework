namespace MASA.Contrib.Dispatcher.InMemory.CheckMethodsParameterNotNull.Tests.Events;

public class DeleteGoodsEvent : Event
{
    public string GoodsId { get; set; }
}