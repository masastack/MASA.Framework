// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; }

    internal bool EnableSoftDelete { get; }

    public DbContextOptionsBuilder DbContextOptionsBuilder { get; }

    public Type DbContextType { get; }

    public MasaDbContextOptionsBuilder(MasaDbContextOptions masaDbContextOptions, Func<DbContextOptionsBuilder>? configure = null)
    {
        ServiceProvider = masaDbContextOptions.ServiceProvider;
        EnableSoftDelete = masaDbContextOptions.EnableSoftDelete;
        DbContextType = masaDbContextOptions.DbContextType;
        DbContextOptionsBuilder = configure == null ? new DbContextOptionsBuilder(masaDbContextOptions) : configure.Invoke();
    }

    public MasaDbContextOptionsBuilder(DbContextOptionsBuilder dbContextOptionsBuilder, MasaDbContextOptions masaDbContextOptions)
        : this(masaDbContextOptions, () => dbContextOptionsBuilder)
    {
    }
}
