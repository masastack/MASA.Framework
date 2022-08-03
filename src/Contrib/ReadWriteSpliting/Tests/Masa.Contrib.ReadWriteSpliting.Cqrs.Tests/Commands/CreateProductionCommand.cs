// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Commands;

namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Tests.Commands;

public record CreateProductionCommand : Command
{
    public string Name { get; set; }

    public int Count { get; set; }
}
