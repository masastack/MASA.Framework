namespace MASA.Contribs.DDD.Domain.Entities;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }
}

