// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : MasaDbContext<TDbContext>, IMasaDbContext
{
    public MasaDbContextOptions<TDbContext> MasaOptions
        => new(ServiceProvider, DbContextOptionsBuilder.Options, EnableSoftDelete, EnablePluralizingTableName);

    public MasaDbContextOptionsBuilder(bool enableSoftDelete = false, bool enablePluralizingTableName = false) : this(null, enableSoftDelete, enablePluralizingTableName)
    {
    }

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete, bool enablePluralizingTableName)
        : base(serviceProvider, new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete, enablePluralizingTableName))
    {
    }
}
