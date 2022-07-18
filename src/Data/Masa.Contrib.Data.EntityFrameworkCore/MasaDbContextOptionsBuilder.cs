// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class MasaDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; set; }

    public bool EnableSoftDelete { get; protected set; }

    public DbContextOptionsBuilder DbContextOptionsBuilder { get; protected set; }

    internal MasaDbContextOptionsBuilder()
    {
    }

    protected MasaDbContextOptionsBuilder(IServiceProvider? serviceProvider, MasaDbContextOptions options)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = options.EnableSoftDelete;
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
    }
}
