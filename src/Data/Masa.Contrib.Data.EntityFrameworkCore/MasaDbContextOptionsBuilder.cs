// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore;

public class MasaDbContextOptionsBuilder
{
    private IServiceProvider? _serviceProvider;

    public IServiceProvider ServiceProvider => _serviceProvider ??= Services.BuildServiceProvider();

    public IServiceCollection Services { get; }

    public Type DbContextType { get; }

    public Action<IServiceProvider, DbContextOptionsBuilder> Builder { get; set; } = default!;

    public bool EnableSoftDelete { get; set; }

    public MasaDbContextOptionsBuilder(IServiceCollection services, Type dbContextType)
    {
        Services = services;
        DbContextType = dbContextType;
    }
}
