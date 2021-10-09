namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class ComputeEvent : Event
{
    /// <summary>
    /// the unit price
    /// </summary>
    public decimal Price { get; set; }

    public int Count { get; set; }

    /// <summary>
    /// original price
    /// </summary>
    public decimal Amount => Price * Count;

    /// <summary>
    /// preferential price
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// actual amount paid
    /// </summary>
    public decimal PayAmount { get; set; }
}