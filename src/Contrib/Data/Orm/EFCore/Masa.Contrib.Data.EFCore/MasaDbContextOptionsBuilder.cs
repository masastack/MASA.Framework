// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder
{
    internal IServiceProvider? ServiceProvider { get; }

    internal bool EnableSoftDelete { get; }

    public virtual DbContextOptionsBuilder DbContextOptionsBuilder { get; }

    public Type DbContextType { get; }

    private MasaDbContextOptionsBuilder(bool enableSoftDelete, Type dbContextType)
    {
        EnableSoftDelete = enableSoftDelete;
        DbContextType = dbContextType;
    }

    public MasaDbContextOptionsBuilder(IServiceProvider? serviceProvider, MasaDbContextOptions options)
        : this(options.EnableSoftDelete, options.DbContextType)
    {
        ServiceProvider = serviceProvider;
        DbContextOptionsBuilder = new DbContextOptionsBuilder(options);
    }

    internal MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        DbContextOptionsBuilder dbContextOptionsBuilder,
        Type dbContextType)
        : this(false, dbContextType)
    {
        DbContextOptionsBuilder = dbContextOptionsBuilder;
    }
}
