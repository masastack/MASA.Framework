// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore;

public class MasaDbContextBuilder
{
    private IServiceProvider? _serviceProvider;

    public IServiceProvider ServiceProvider => _serviceProvider ??= Services.BuildServiceProvider();

    public IServiceCollection Services { get; }

    public Type DbContextType { get; }

    public Type UserIdType { get; }

    public Action<IServiceProvider, DbContextOptionsBuilder> Builder { get; set; } = default!;

    public bool EnableSoftDelete { get; set; }

    public MasaDbContextBuilder(IServiceCollection services, Type dbContextType, Type userIdType)
    {
        Services = services;
        DbContextType = dbContextType;
        UserIdType = userIdType;
    }
}
