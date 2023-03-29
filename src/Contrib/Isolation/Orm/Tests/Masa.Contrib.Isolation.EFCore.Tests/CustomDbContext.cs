// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Data.EFCore.Tests.Scenes.Isolation;

namespace Masa.Contrib.Isolation.EFCore.Tests;

public class CustomDbContext : IsolationDbContext<CustomDbContext, int>
{
    public DbSet<User> User { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
