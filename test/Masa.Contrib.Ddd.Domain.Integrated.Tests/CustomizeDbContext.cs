// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Integrated.Tests;

public class CustomizeDbContext : MasaDbContext<CustomizeDbContext>
{
    public DbSet<User> User { get; set; }

    public CustomizeDbContext(MasaDbContextOptions<CustomizeDbContext> options) : base(options)
    {
    }
}
