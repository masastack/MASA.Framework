// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

public class CustomDbContext : MasaDbContext<CustomDbContext, int>
{
    public DbSet<User> User { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}

public class CustomDbContext2 : MasaDbContext
{
    public DbSet<Order> Order { get; set; }

    public CustomDbContext2(MasaDbContextOptions<CustomDbContext2> options) : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
    }
}

public class CustomDbContext3 : MasaDbContext
{
    public DbSet<Order> Order { get; set; }

    protected override void OnConfiguring(MasaDbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = "data source=customDbContext3";
        optionsBuilder.UseSqlite(connectionString);

        optionsBuilder.DbContextOptionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution);
    }
}

public class CustomDbContext4 : MasaDbContext
{
    public DbSet<Order> Order { get; set; }
}
