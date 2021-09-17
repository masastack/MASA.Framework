namespace MASA.Contrib.Dispatcher.InMemory.OrderEqualBySaga.Tests.Events;

public class EditCategoryEvent : Event
{
    public string CategoryId { get; set; }

    public string CategoryName { get; set; }
}