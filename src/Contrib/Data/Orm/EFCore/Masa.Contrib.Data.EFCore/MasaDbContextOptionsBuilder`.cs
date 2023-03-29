// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : DbContext, IMasaDbContext
{
    public MasaDbContextOptions<TDbContext> MasaOptions
        => new(ServiceProvider, DbContextOptionsBuilder.Options, EnableSoftDelete, EnablePluarlizingTableName);

    public MasaDbContextOptionsBuilder(bool enableSoftDelete = false, bool enablePluarlizingTableName = false) : this(null, enableSoftDelete, enablePluarlizingTableName)
    {
    }

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete, bool enablePluarlizingTableName)
        : base(serviceProvider, new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete, enablePluarlizingTableName))
    {
    }
}
