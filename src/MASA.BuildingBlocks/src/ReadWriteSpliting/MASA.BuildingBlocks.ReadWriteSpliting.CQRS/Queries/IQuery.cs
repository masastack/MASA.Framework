namespace MASA.BuildingBlocks.ReadWriteSpliting.CQRS.Queries;
public interface IQuery<TResult> : IEvent<TResult>
    where TResult : notnull
{
}