// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Oidc.EntityFramework.DbContexts;

public class OidcDbContext : DbContext
{
    public OidcDbContext(DbContextOptions<OidcDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OidcDbContext).Assembly);
    }
}
