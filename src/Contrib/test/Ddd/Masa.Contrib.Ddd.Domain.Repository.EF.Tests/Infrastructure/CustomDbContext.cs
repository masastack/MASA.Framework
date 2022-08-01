// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Infrastructure;

public class CustomDbContext : MasaDbContext
{
    public DbSet<Orders> Orders { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options) { }
}
