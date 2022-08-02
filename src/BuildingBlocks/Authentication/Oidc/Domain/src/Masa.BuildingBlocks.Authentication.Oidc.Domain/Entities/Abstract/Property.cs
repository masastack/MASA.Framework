// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities.Abstract;

public abstract class Property : Entity<Guid>
{
    public string Key { get; protected set; } = "";

    public string Value { get; protected set; } = "";
}
