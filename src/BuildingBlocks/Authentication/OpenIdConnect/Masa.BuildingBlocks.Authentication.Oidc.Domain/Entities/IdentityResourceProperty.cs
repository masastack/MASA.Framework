// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Entities
{
    public class IdentityResourceProperty : Property
    {
        public int IdentityResourceId { get; private set; }

        public IdentityResource IdentityResource { get; private set; } = null!;

        public IdentityResourceProperty(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
