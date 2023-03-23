// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using WebApplication1.Infrastructure.Model;

namespace WebApplication1.Infrastructure;

public class UserDbContext : MasaDbContext<UserDbContext>
{
    public DbSet<User> User { get; set; }

    public UserDbContext(MasaDbContextOptions<UserDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguringExecuting(MasaDbContextOptionsBuilder<UserDbContext> modelBuilder)
    {
        modelBuilder.UseSqlite("");
    }
}
