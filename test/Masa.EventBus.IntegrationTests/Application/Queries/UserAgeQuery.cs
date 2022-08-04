// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.EventBus.IntegrationTests.Application.Queries;

public record UserAgeQuery : Query<int>
{
    public string Name { get; set; }

    public override int Result { get; set; }
}
