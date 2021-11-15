namespace MASA.Contrib.Data.Contracts.EF.Test.Domain.Entities;

public class Students : AuditAggregateRoot<Guid, Guid>
{
    public Students()
    {
        this.Id = Guid.NewGuid();
        this.RegisterTime = DateTime.UtcNow;
    }

    public string Name { get; set; }

    public int Age { get; set; }

    public DateTime RegisterTime { get; private set; }
}
