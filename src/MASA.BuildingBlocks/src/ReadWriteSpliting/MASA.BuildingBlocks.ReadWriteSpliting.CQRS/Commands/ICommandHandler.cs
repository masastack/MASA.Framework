namespace MASA.BuildingBlocks.ReadWriteSpliting.CQRS.Commands;
public interface ICommandHandler<TCommand> : IEventHandler<TCommand>
    where TCommand : ICommand
{
}