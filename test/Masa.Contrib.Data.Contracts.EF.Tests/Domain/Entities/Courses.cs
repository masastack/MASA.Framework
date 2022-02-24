namespace Masa.Contrib.Data.Contracts.EF.Tests.Domain.Entities;

public class Courses : AggregateRoot<Guid>
{
    public Courses()
    {
        Id = Guid.NewGuid();
        IsDeleted = false;
    }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }
}
