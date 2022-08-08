// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Infrastructure;

public class CustomDbContext : MasaDbContext
{
    public DbSet<User> User { get; set; }

    public CustomDbContext(MasaDbContextOptions options) : base(options)
    {
    }
}
