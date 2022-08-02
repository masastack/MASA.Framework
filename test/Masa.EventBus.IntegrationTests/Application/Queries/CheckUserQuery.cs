// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.EventBus.IntegrationTests.Application.Queries;

public record CheckUserQuery : Query<bool>
{
    public string Name { get; set; }

    public override bool Result { get; set; }
}
