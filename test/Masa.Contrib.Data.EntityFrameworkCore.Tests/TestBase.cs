// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EntityFrameworkCore.Tests;

public class TestBase
{
    protected IServiceCollection Services;

    [TestInitialize]
    public void Initialize()
    {
        Services = new ServiceCollection();
    }

    protected CustomizeDbContext CreateDbContext(bool enableSoftDelete, out IServiceProvider serviceProvider,
        bool initConnectionString = true)
    {
        Services.AddMasaDbContext<CustomizeDbContext>(options =>
        {
            if (enableSoftDelete)
                options.UseTestFilter();

            if (initConnectionString)
                options.UseTestSqlite($"data source=test-{Guid.NewGuid()}");
            else
                options.UseSqlite();
        });
        serviceProvider = Services.BuildServiceProvider();
        var dbContext = serviceProvider.GetRequiredService<CustomizeDbContext>();
        dbContext.Database.EnsureCreated();
        return dbContext;
    }
}
