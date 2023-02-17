// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

public class TestBase
{
    protected IServiceCollection Services;

    public TestBase()
    {
        Services = new ServiceCollection();
    }

    protected CustomDbContext CreateDbContext(bool enableSoftDelete, out IServiceProvider serviceProvider)
    {
        Services.AddMasaDbContext<CustomDbContext>(options =>
        {
            if (enableSoftDelete)
                options.UseFilter();

            options.UseSqlite($"data source=test-{Guid.NewGuid()}");
        });
        serviceProvider = Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomDbContext>();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }

    protected CustomQueryDbContext CreateQueryDbContext(bool enableSoftDelete, out IServiceProvider serviceProvider)
    {
        Services.AddMasaDbContext<CustomQueryDbContext>(options =>
        {
            if (enableSoftDelete)
                options.UseFilter();

            options.UseSqlite($"data source=test2-{Guid.NewGuid()}");
        });
        serviceProvider = Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomQueryDbContext>();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}
