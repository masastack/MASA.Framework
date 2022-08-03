// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Identity.IdentityModel;

public class IdentityUser : IIdentityUser
{
    public string Id { get; set; }

    public string? UserName { get; set; }

    public IEnumerable<IdentityRole<string>> Roles { get; set; } = new List<IdentityRole<string>>();
}
