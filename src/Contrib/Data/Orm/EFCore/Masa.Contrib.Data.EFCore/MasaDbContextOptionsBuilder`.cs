// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : MasaDbContext, IMasaDbContext
{
    public MasaDbContextOptions<TDbContext> MasaOptions
        => new(ServiceProvider, (DbContextOptions<TDbContext>)DbContextOptionsBuilder.Options, EnableSoftDelete);

    public MasaDbContextOptionsBuilder(bool enableSoftDelete = false) : this(null, enableSoftDelete)
    {
    }

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete)
        : base(serviceProvider, new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete))
    {
    }
}
