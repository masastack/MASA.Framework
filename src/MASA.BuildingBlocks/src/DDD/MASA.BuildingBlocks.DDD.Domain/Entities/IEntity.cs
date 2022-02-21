namespace MASA.BuildingBlocks.DDD.Domain.Entities;
public interface IEntity
{
    IEnumerable<(string Name, object Value)> GetKeys();
}

public interface IEntity<TKey> : IEntity
{
    TKey Id { get; set; }
}
