namespace MASA.Contrib.Dispatcher.InMemory.OrderLessThanZeroBySaga.Tests.Events;

public class EditGoodsEvent : Event
{
    public string GoodsId { get; set; }

    public string CategoryId { get; set; }

    public string GoodsName { get; set; }
}