﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests.Infrastructure;

public class CustomizeDbContext : MasaDbContext
{
    public DbSet<User> User { get; set; }

    public CustomizeDbContext(MasaDbContextOptions options) : base(options)
    {
    }
}
