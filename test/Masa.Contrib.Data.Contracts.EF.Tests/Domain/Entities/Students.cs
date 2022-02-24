namespace Masa.Contrib.Data.Contracts.EF.Tests.Domain.Entities;

public class Students : AuditAggregateRoot<Guid, Guid>
{
    public Students()
    {
        Id = Guid.NewGuid();
        RegisterTime = DateTime.UtcNow;
    }

    public string Name { get; set; }

    public int Age { get; set; }

    public DateTime RegisterTime { get; private set; }
}
