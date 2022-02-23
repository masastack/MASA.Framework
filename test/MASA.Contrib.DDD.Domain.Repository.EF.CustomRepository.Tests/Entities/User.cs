namespace MASA.Contrib.DDD.Domain.Repository.EF.CustomRepository.Tests.Entities;

public class User : AggregateRoot<Guid>
{
    public string Name { get; set; }

    public bool Gender { get; set; }
}
