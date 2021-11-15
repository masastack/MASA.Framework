namespace MASA.Contribs.DDD.Domain.Entities;

public class Users : AggregateRoot<Guid>
{
    public string Name { get; set; }
}

