// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ClientCorsOrigin : Entity<Guid>
{
    public string Origin { get; private set; } = string.Empty;

    public Guid ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientCorsOrigin(string origin)
    {
        Origin = origin;
    }
}
