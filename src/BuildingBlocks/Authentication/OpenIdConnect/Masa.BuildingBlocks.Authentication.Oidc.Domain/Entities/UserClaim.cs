// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities;

public class UserClaim : FullAggregateRoot<int, Guid>
{
    public string Name { get; private set; }

    public string Description { get; private set; }

    public UserClaim(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public void Update(string description)
    {
        Description = description;
    }
}
