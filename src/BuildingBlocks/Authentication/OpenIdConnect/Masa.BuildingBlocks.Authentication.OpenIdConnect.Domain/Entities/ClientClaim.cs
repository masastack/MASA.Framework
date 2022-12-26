// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ClientClaim : Entity<Guid>
{
    public string Type { get; private set; }

    public string Value { get; private set; }

    public Guid ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientClaim(string type, string value)
    {
        Type = type;
        Value = value;
    }
}
