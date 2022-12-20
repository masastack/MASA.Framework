// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.Tests;

public class CustomUser : IIdentityUser
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public string? TrueName { get; set; }

    public string[] Roles { get; set; }
}
