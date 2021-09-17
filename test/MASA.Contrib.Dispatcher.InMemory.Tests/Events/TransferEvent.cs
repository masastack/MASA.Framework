namespace MASA.Contrib.Dispatcher.InMemory.Tests.Events;

public class TransferEvent : Event
{
    public string Account { get; set; }

    public string OptAccount { get; set; }

    public decimal Price { get; set; }

    public override string ToString()
    {
        return $"Account:{Account}, OptAccount:{OptAccount}, Price:{Price}, " + base.ToString();
    }
}