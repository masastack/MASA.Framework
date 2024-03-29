// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ClientProperty : Property
{
    public Guid ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientProperty(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
