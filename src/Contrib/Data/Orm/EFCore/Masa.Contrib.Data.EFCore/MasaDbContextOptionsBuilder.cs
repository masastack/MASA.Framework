// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public abstract class MasaDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; }

    public bool EnableSoftDelete { get; }

    public bool EnablePluralizingTableName { get; }

    public virtual DbContextOptionsBuilder DbContextOptionsBuilder { get; }

    protected MasaDbContextOptionsBuilder(IServiceProvider? serviceProvider, MasaDbContextOptions options)
    {
        ServiceProvider = serviceProvider;
        EnableSoftDelete = options.EnableSoftDelete;
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
    }
}
