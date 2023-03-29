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
