// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Application.Command;

public record RegisterUserCommand : BuildingBlocks.ReadWriteSplitting.Cqrs.Commands.Command
{
    public string Name { get; set; }

    public int Age { get; set; }
}
