namespace MASA.Contrib.DDD.Domain.Repository.EF.CombinedKeysNoFind.Tests;

public class Courses : AggregateRoot
{
    public Courses()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; init; }

    public string Name { get; set; }

    public override IEnumerable<(string Name, object Value)> GetKeys()
        => new List<(string Name, object Value)>()
        {
                ("Names",Name)//Demonstrate that a non-existent key is used as a joint primary key
        };
}
