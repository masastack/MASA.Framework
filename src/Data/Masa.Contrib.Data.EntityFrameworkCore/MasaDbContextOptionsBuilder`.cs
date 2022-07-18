// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder<TDbContext> : MasaDbContextOptionsBuilder
    where TDbContext : MasaDbContext, IMasaDbContext
{
    public IServiceCollection Services { get; } = new ServiceCollection();

    public MasaDbContextOptions<TDbContext> MasaOptions
        => new(ServiceProvider, (DbContextOptions<TDbContext>)DbContextOptionsBuilder.Options, EnableSoftDelete);

    public MasaDbContextOptionsBuilder(Action<MasaDbContextOptionsBuilder<TDbContext>>? action = null, bool enableSoftDelete = false)
    {
        action?.Invoke(this);
        ServiceProvider = Services.BuildServiceProvider();
        EnableSoftDelete = enableSoftDelete;
        DbContextOptionsBuilder =
            new DbContextOptionsBuilder(
                new MasaDbContextOptions<TDbContext>(ServiceProvider, new DbContextOptions<TDbContext>(),
                    enableSoftDelete));
    }

    public MasaDbContextOptionsBuilder(
        IServiceProvider? serviceProvider,
        bool enableSoftDelete)
        : base(serviceProvider, new MasaDbContextOptions<TDbContext>(serviceProvider, new DbContextOptions<TDbContext>(), enableSoftDelete))
    {
    }
}
