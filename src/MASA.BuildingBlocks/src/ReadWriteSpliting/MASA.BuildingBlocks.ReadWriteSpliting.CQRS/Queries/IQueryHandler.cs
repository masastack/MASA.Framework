namespace MASA.BuildingBlocks.ReadWriteSpliting.CQRS.Queries;
public interface IQueryHandler<TCommand, TResult> : IEventHandler<TCommand>
    where TCommand : IQuery<TResult> where TResult : notnull
{
}
