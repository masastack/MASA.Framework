namespace Masa.Contrib.Data.UoW.EF;

public class Transaction : ITransaction
{
    public Transaction(IUnitOfWork unitOfWork) => UnitOfWork = unitOfWork;

    [JsonIgnore]
    public IUnitOfWork? UnitOfWork { get; set; }
}
