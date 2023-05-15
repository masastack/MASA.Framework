// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests;

public class TestBase
{
    protected static string MemoryConnectionString => $"test-{Guid.NewGuid()}";

    protected readonly IServiceCollection Services;

    protected TestBase()
    {
        Services = new ServiceCollection();
        Services.InitializeCacheData();
    }

    protected DefaultMasaDbContext CreateDbContext<TDbContext>(
        Action<MasaDbContextBuilder>? optionsBuilder,
        Action<IServiceProvider>? configure = null)
        where TDbContext : DefaultMasaDbContext, ICustomDbContext
    {
        Services.AddMasaDbContext<TDbContext>(optionsBuilder);

        var serviceScope = Services.BuildServiceProvider().CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var dbContext = serviceProvider.GetService<TDbContext>();

        VerifyDbContext(dbContext);
        dbContext.Database.EnsureCreated();
        configure?.Invoke(serviceProvider);
        return dbContext;
    }

    protected async Task<DefaultMasaDbContext> CreateDbContextAsync<TDbContext>(
        Action<MasaDbContextBuilder>? optionsBuilder,
        Action<IServiceProvider>? configure = null)
        where TDbContext : DefaultMasaDbContext, ICustomDbContext
    {
        Services.AddMasaDbContext<TDbContext>(optionsBuilder);
        var serviceScope = Services.BuildServiceProvider().CreateAsyncScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var dbContext = serviceProvider.GetService<TDbContext>();

        VerifyDbContext(dbContext);
        await dbContext.Database.EnsureCreatedAsync();
        configure?.Invoke(serviceProvider);
        return dbContext;
    }

    private static void VerifyDbContext<TDbContext>([NotNull] TDbContext? dbContext) where TDbContext : ICustomDbContext
    {
        Assert.IsNotNull(dbContext);
        Assert.AreEqual(dbContext.GetType().Name, dbContext.Name);
    }
}
