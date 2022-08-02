// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Domain.Constants;

public class GrantTypeConsts
{
    public static ICollection<string> Implicit =>
            new[] { GrantTypes.IMPLICIT };

    public static ICollection<string> ImplicitAndClientCredentials =>
        new[] { GrantTypes.IMPLICIT, GrantTypes.CLIENT_CREDENTIALS };

    public static ICollection<string> Code =>
        new[] { GrantTypes.AUTHORIZATION_CODE };

    public static ICollection<string> CodeAndClientCredentials =>
        new[] { GrantTypes.AUTHORIZATION_CODE, GrantTypes.CLIENT_CREDENTIALS };

    public static ICollection<string> Hybrid =>
        new[] { GrantTypes.HYBRID };

    public static ICollection<string> HybridAndClientCredentials =>
        new[] { GrantTypes.HYBRID, GrantTypes.CLIENT_CREDENTIALS };

    public static ICollection<string> ClientCredentials =>
        new[] { GrantTypes.CLIENT_CREDENTIALS };

    public static ICollection<string> ResourceOwnerPassword =>
        new[] { GrantTypes.RESOURCE_OWNER_PASSWORD };

    public static ICollection<string> ResourceOwnerPasswordAndClientCredentials =>
        new[] { GrantTypes.RESOURCE_OWNER_PASSWORD, GrantTypes.CLIENT_CREDENTIALS };

    public static ICollection<string> DeviceFlow =>
        new[] { GrantTypes.DEVICE_FLOW };
}
