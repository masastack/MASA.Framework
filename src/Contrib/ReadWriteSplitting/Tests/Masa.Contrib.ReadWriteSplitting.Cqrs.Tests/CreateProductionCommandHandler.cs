// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Tests;

public class CreateProductionCommandHandler : CommandHandler<CreateProductionCommand>
{
    [EventHandler(1, Dispatcher.Events.Enums.FailureLevels.ThrowAndCancel, false)]
    public override Task HandleAsync(CreateProductionCommand @event, CancellationToken cancellationToken = default)
    {
        @event.Count++;
        if (string.IsNullOrEmpty(@event.Name))
            throw new ArgumentNullException(nameof(@event));

        if (@event.GetEventId() == default(Guid) || @event.GetCreationTime() > DateTime.UtcNow)
            throw new ArgumentNullException(nameof(@event));

        return Task.CompletedTask;
    }

    [EventHandler(1)]
    public override Task CancelAsync(CreateProductionCommand @event, CancellationToken cancellationToken = default)
    {
        @event.Count++;
        return base.CancelAsync(@event, cancellationToken);
    }
}
