namespace MASA.Contrib.ReadWriteSpliting.CQRS.Tests;

public class CreateProductionCommandHandler : CommandHandler<CreateProductionCommand>
{
    [EventHandler(1, Dispatcher.Events.Enums.FailureLevels.ThrowAndCancel, false)]
    public override Task HandleAsync(CreateProductionCommand @event)
    {
        @event.Count++;
        if (string.IsNullOrEmpty(@event.Name))
            throw new ArgumentNullException(nameof(@event));

        if (@event.Id == default(Guid) || @event.CreationTime > DateTime.UtcNow)
            throw new ArgumentNullException(nameof(@event));

        return Task.CompletedTask;
    }

    [EventHandler(1)]
    public override Task CancelAsync(CreateProductionCommand @event)
    {
        @event.Count++;
        return base.CancelAsync(@event);
    }
}
