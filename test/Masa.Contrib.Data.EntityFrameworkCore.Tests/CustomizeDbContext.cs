// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Tests;

public class CustomizeDbContext : MasaDbContext<CustomizeDbContext>
{
    public CustomizeDbContext(MasaDbContextOptions<CustomizeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>();
        modelBuilder.Entity<Student>().OwnsOne(x => x.Address);
        modelBuilder.Entity<Student>().OwnsMany(t => t.Hobbies);
    }
}
