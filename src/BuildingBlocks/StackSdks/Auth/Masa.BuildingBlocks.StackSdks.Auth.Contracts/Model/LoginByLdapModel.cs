// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

public class LoginByLdapModel
{
    public string ClientId { get; set; } = "";

    public string ClientSecret { get; set; } = "";

    public List<string> Scope { get; set; } = new() { "openid", "profile" };

    public string UserName { get; set; }

    public string Scheme { get; set; } = "Ldap";
}
