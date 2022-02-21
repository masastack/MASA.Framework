namespace MASA.BuildingBlocks.DDD.Domain.Events;
public interface IDomainQuery<TResult> : IDomainEvent, IQuery<TResult>
    where TResult : notnull
{

}
