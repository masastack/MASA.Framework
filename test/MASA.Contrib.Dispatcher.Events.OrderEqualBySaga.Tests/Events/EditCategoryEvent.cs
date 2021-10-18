namespace MASA.Contrib.Dispatcher.Events.OrderEqualBySaga.Tests.Events;

public record EditCategoryEvent : Event
{
    public string CategoryId { get; set; }

    public string CategoryName { get; set; }
}
