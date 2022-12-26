// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ClientScope : Entity<Guid>
{
    public string Scope { get; private set; }

    public Guid ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientScope(string scope)
    {
        Scope = scope;
    }
}
