// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public abstract class MasaDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; }

    public bool EnableSoftDelete { get; }

    public DbContextOptionsBuilder DbContextOptionsBuilder { get; }

    protected MasaDbContextOptionsBuilder(IServiceProvider? serviceProvider, MasaDbContextOptions options)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = options.EnableSoftDelete;
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
    }
}
