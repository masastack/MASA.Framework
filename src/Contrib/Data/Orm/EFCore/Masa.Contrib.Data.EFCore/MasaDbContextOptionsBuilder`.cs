// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : DbContext, IMasaDbContext
{
    public MasaDbContextOptions<TDbContext> MasaOptions
        => new(ServiceProvider, DbContextOptionsBuilder.Options, EnableSoftDelete);

    public MasaDbContextOptionsBuilder(bool enableSoftDelete = false) : this(null, enableSoftDelete)
    {
    }

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete = false)
        : base(new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete))
    {
    }
}
