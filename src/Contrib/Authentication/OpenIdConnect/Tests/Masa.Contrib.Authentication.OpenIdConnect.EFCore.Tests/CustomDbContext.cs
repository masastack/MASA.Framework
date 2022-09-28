// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Tests;

public class CustomDbContext : IsolationDbContext
{
    public DbSet<UserClaim> UserClaims { get; set; }

    public DbSet<IdentityResource> IdentityResources { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {

    }
}
