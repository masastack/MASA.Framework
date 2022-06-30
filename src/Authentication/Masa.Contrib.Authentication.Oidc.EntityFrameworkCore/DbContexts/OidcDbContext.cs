// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Oidc.EntityFrameworkCore.DbContexts;

public class OidcDbContext
{
    public DbContext Dbcontext { get; set; }

    public OidcDbContext(DbContext dbcontext)
    {
        Dbcontext = dbcontext;
    }
}
