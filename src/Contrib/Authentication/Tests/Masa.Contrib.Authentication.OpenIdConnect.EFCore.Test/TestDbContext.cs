// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.OpenIdConnect.EFCore.Test;

public class TestDbContext : IsolationDbContext
{
    public TestDbContext(MasaDbContextOptions<TestDbContext> options) : base(options)
    {

    }

    public DbSet<UserClaim> UserClaims { get; set; }

    public DbSet<IdentityResource> IdentityResources { get; set; }
}
