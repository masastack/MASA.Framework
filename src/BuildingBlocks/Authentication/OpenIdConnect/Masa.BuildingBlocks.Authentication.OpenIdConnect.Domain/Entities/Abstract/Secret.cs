// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities.Abstract;

public abstract class Secret : FullEntity<int, Guid>
{
    public string Description { get; protected set; } = string.Empty;

    public string Value { get; protected set; } = string.Empty;

    public DateTime? Expiration { get; protected set; }

    public string Type { get; protected set; } = "SharedSecret";
}
