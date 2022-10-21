// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;

public class ApiScopeProperty : Property
{
    public Guid ScopeId { get; private set; }

    public ApiScope Scope { get; private set; } = null!;

    public ApiScopeProperty(string key, string value)
    {
        Key = key;
        Value = value;
    }
}
