// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ClientRedirectUri : Entity<Guid>
{
    public string RedirectUri { get; private set; } = string.Empty;

    public Guid ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientRedirectUri(string redirectUri)
    {
        RedirectUri = redirectUri;
    }
}

