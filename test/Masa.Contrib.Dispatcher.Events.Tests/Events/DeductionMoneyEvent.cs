namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record DeductionMoneyEvent : Event, ITransaction
{
    public IUnitOfWork? UnitOfWork { get; set; }

    public string Account { get; set; }

    public string PayeeAccount { get; set; }

    public decimal Money { get; set; }
}
