namespace MASA.Contrib.Dispatcher.Events.Tests.Events;

public record IncreaseMoneyEvent : Event, ITransaction
{
    public IUnitOfWork? UnitOfWork { get; set; }

    public string Account { get; set; }

    public string TransferAccount { get; set; }

    public decimal Money { get; set; }
}
