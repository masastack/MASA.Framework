// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.Oidc.Models.Constans;

public class GrantTypes
{
    public static ICollection<string> Implicit =>
        new[] { GrantType.IMPLICIT };

    public static ICollection<string> ImplicitAndClientCredentials =>
        new[] { GrantType.IMPLICIT, GrantType.CLIENT_CREDENTIALS };

    public static ICollection<string> Code =>
        new[] { GrantType.AUTHORIZATION_CODE };

    public static ICollection<string> CodeAndClientCredentials =>
        new[] { GrantType.AUTHORIZATION_CODE, GrantType.CLIENT_CREDENTIALS };

    public static ICollection<string> Hybrid =>
        new[] { GrantType.HYBRID };

    public static ICollection<string> HybridAndClientCredentials =>
        new[] { GrantType.HYBRID, GrantType.CLIENT_CREDENTIALS };

    public static ICollection<string> ClientCredentials =>
        new[] { GrantType.CLIENT_CREDENTIALS };

    public static ICollection<string> ResourceOwnerPassword =>
        new[] { GrantType.RESOURCE_OWNER_PASSWORD };

    public static ICollection<string> ResourceOwnerPasswordAndClientCredentials =>
        new[] { GrantType.RESOURCE_OWNER_PASSWORD, GrantType.CLIENT_CREDENTIALS };

    public static ICollection<string> DeviceFlow =>
        new[] { GrantType.DEVICE_FLOW };
}
