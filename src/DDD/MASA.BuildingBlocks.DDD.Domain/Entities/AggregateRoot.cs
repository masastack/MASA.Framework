namespace MASA.BuildingBlocks.DDD.Domain.Entities;
public abstract class AggregateRoot : Entity, IAggregateRoot
{

}

public class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot<TKey>
{

}
