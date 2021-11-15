namespace MASA.Contrib.Data.Contracts.EF.Test.Domain.Entities;

public class Courses : AggregateRoot<Guid>
{
    public Courses()
    {
        Id = Guid.NewGuid();
        IsDeleted = false;
    }

    public Guid Id { get; init; }

    public string Name { get; set; }

    public bool IsDeleted { get; set; }
}
