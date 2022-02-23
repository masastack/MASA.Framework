namespace MASA.Contrib.DDD.Domain.Repository.EF.Entity.Tests;

public class Hobbies : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    private Hobbies()
    {
        Id = Guid.NewGuid();
    }

    public Hobbies(string name) : this()
    {
        Name = name;
    }
}
