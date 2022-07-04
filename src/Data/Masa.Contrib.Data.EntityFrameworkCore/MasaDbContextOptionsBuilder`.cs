// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : MasaDbContext, IMasaDbContext
{
    public MasaDbContextOptions<TDbContext> MasaDbContextOptions
        => new(ServiceProvider, (DbContextOptions<TDbContext>)DbContextOptionsBuilder.Options, EnableSoftDelete);

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete)
        : base(serviceProvider, new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete))
    {
    }
}
