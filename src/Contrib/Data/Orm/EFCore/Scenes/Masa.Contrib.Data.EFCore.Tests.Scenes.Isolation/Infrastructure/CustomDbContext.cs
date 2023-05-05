// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

public class CustomDbContext : MasaDbContext<CustomDbContext, int>
{
    public DbSet<User> User { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}

public class CustomDbContext2 : MasaDbContext<CustomDbContext2>
{
    public DbSet<Order> Order { get; set; }

    public CustomDbContext2(MasaDbContextOptions<CustomDbContext2> options) : base(options)
    {
        base.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;
    }
}
