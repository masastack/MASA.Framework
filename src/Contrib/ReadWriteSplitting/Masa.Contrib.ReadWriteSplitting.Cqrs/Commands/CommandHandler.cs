// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSplitting.Cqrs.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>, ISagaEventHandler<TCommand>
    where TCommand : ICommand
{
    public abstract Task HandleAsync(TCommand @event, CancellationToken cancellationToken = default);



    public virtual Task CancelAsync(TCommand @event, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
