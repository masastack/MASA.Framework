// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Commands;

public interface ICommandHandler<TCommand> : IEventHandler<TCommand>
    where TCommand : ICommand
{
}
