namespace MASA.Contrib.Dispatcher.InMemory.CheckMethodsParameter.Tests.Events;

public class AddGoodsEvent : Event
{
    public string GoodsId { get; set; }

    public string CategoryId { get; set; }

    public string GoodsName { get; set; }
}
