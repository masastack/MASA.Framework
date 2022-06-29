// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.ReadWriteSpliting.Cqrs.Queries;

namespace Masa.Contrib.ReadWriteSpliting.Cqrs.Tests.Queries;

public record ProductionItemQuery : Query<string>
{
    public override string Result { get; set; }

    public string ProductionId { get; set; }
}
