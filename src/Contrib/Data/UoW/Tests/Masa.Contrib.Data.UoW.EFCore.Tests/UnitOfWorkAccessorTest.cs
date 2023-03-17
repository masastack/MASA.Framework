// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore.Tests;

[TestClass]
public class UnitOfWorkAccessorTest : TestBase
{
    private Mock<IDispatcherOptions> _options;

    [TestInitialize]
    public void Initialize()
    {
        _options = new();
        _options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
    }

    [TestMethod]
    public async Task TestUnitOfWorkAccessorAsync()
    {
        var services = new ServiceCollection();
        services.Configure<ConnectionStrings>(options =>
        {
            options.DefaultConnection = _connectionString;
        });

        _options.Setup(option => option.Services).Returns(services).Verifiable();
        _options.Object.UseUoW<CustomDbContext>(options => options.UseSqlite());
        var serviceProvider = _options.Object.Services.BuildServiceProvider();
        var unitOfWorkAccessor = serviceProvider.GetService<IUnitOfWorkAccessor>();
        Assert.IsNotNull(unitOfWorkAccessor.CurrentDbContextOptions);
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        Assert.IsNotNull(unitOfWork);
        Assert.IsTrue(!unitOfWork.TransactionHasBegun);
        unitOfWorkAccessor = serviceProvider.GetService<IUnitOfWorkAccessor>();
        Assert.IsTrue(unitOfWorkAccessor!.CurrentDbContextOptions != null &&
            unitOfWorkAccessor.CurrentDbContextOptions.TryGetConnectionString(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, out string? connectionString)
            && connectionString == _connectionString);

        var unitOfWorkManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
        var unitOfWorkNew = unitOfWorkManager.CreateDbContext(false);
        var unitOfWorkAccessorNew = unitOfWorkNew.ServiceProvider.GetService<IUnitOfWorkAccessor>();
        Assert.IsTrue(unitOfWorkAccessorNew!.CurrentDbContextOptions != null &&
            unitOfWorkAccessorNew.CurrentDbContextOptions.TryGetConnectionString(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, out connectionString) &&
            connectionString == _connectionString);

        var unitOfWorkNew2 = unitOfWorkManager.CreateDbContext(new DbContextConnectionStringOptions("test"));
        var unitOfWorkAccessorNew2 = unitOfWorkNew2.ServiceProvider.GetService<IUnitOfWorkAccessor>();
        Assert.IsTrue(
            unitOfWorkAccessorNew2!.CurrentDbContextOptions != null &&
            unitOfWorkAccessorNew2.CurrentDbContextOptions.TryGetConnectionString(ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME, out connectionString) &&
            connectionString == "test");

        connectionString = await unitOfWorkNew2.ServiceProvider.GetRequiredService<IConnectionStringProvider>().GetConnectionStringAsync();
        Assert.IsTrue(connectionString == "test");
    }
}
