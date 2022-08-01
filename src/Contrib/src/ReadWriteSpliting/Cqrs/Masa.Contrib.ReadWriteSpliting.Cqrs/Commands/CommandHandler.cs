// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>, ISagaEventHandler<TCommand>
    where TCommand : ICommand
{
    public abstract Task HandleAsync(TCommand @event);

    public virtual Task CancelAsync(TCommand @event)
    {
        return Task.CompletedTask;
    }
}
