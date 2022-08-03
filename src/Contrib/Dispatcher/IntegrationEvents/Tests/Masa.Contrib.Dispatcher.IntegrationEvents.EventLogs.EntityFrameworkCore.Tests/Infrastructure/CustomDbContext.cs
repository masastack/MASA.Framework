// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore.Tests.Infrastructure;

internal class CustomDbContext : MasaDbContext
{
    public DbSet<User> Users { get; set; } = null!;

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options)
    {
    }
}
