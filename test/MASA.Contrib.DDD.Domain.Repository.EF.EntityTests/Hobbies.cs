namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests.Domain.Entities;

public class Hobbies : AggregateRoot<Guid>
{
    public string Name { get; private set; }

    private Hobbies()
    {
        this.Id = Guid.NewGuid();
    }

    public Hobbies(string name) : this()
    {
        this.Name = name;
    }
}
