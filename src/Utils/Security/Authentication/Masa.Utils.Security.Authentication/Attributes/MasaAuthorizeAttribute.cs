// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Security.Authentication.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class MasaAuthorizeAttribute : AuthorizeAttribute
{
    public string[] Permissions { get; set; }

    public MasaAuthorizeAttribute(params string[] permissions)
    {
        Permissions = permissions;
    }
}
