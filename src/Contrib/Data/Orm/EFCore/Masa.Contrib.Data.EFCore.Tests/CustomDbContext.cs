// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

public class CustomDbContext : MasaDbContext<CustomDbContext>
{
    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>();
        modelBuilder.Entity<Student>().OwnsOne(x => x.Address);
        modelBuilder.Entity<Student>().OwnsMany(t => t.Hobbies);

        modelBuilder.Entity<User>();
    }
}

public class CustomQueryDbContext : MasaDbContext<CustomQueryDbContext>
{
    public CustomQueryDbContext(MasaDbContextOptions<CustomQueryDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreatingExecuting(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>();
        modelBuilder.Entity<Student>().OwnsOne(x => x.Address);
        modelBuilder.Entity<Student>().OwnsMany(t => t.Hobbies);
    }
}
