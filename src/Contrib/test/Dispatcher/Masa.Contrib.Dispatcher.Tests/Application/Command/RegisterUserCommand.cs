// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests.Application.Command;

public record RegisterUserCommand : BuildingBlocks.ReadWriteSpliting.Cqrs.Commands.Command
{
    public string Name { get; set; }

    public int Age { get; set; }
}
