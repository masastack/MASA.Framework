// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests;

[TestClass]
public class BaseRepositoryTest : TestBase
{
    private IServiceCollection _services = default!;
    private Assembly[] _assemblies;
    private Mock<IUnitOfWork> _uoW;
    private Mock<IDispatcherOptions> _dispatcherOptions = default!;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _assemblies = new Assembly[1]
        {
            typeof(BaseRepositoryTest).Assembly
        };
        _uoW = new();
        _dispatcherOptions = new();
        _dispatcherOptions.Setup(options => options.Services).Returns(() => _services);
    }

    [TestMethod]
    public void TestNullServices()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            _dispatcherOptions.Setup(options => options.Services).Returns(() => null!);
            var options = _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        });
    }

    [TestMethod]
    public void TestUseCustomRepositoryAndNotImplementation()
    {
        Mock<IUnitOfWork> uoW = new();
        _services.AddScoped(_ => uoW.Object);

        Assembly[] assemblies = { typeof(TestBase).Assembly, typeof(IUserRepository).Assembly };
        _dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        Assert.ThrowsException<NotSupportedException>(()
            => _dispatcherOptions.Object.UseRepository<CustomDbContext>()
        );
    }

    [TestMethod]
    public void TestNullUnitOfWork()
    {
        _dispatcherOptions.Setup(option => option.Assemblies).Returns(_assemblies).Verifiable();
        var ex = Assert.ThrowsException<Exception>(() =>
        {
            _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        });
        Assert.IsTrue(ex.Message == "Please add UoW first.");
    }

    [TestMethod]
    public void TestNullAssembly()
    {
        _services.AddScoped(typeof(IUnitOfWork), _ => _uoW.Object);
        _services.AddDbContext<CustomDbContext>(options => options.UseSqlite(Connection));

        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            _dispatcherOptions.Object.UseRepository<CustomDbContext>();
        });
    }

    [TestMethod]
    public void TestAddMultRepository()
    {
        _dispatcherOptions.Setup(option => option.Assemblies).Returns(_assemblies).Verifiable();
        _services.AddScoped(typeof(IUnitOfWork), _ => _uoW.Object);
        _services.AddMasaDbContext<CustomDbContext>(options => options.UseSqlite(Connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>().UseRepository<CustomDbContext>();

        var serviceProvider = _services.BuildServiceProvider();
        var repository = serviceProvider.GetServices<IRepository<Orders>>();
        Assert.IsTrue(repository.Count() == 1);
    }

    [TestMethod]
    public void TestEntityRepositoryShouldBeExist()
    {
        _dispatcherOptions.Setup(option => option.Assemblies).Returns(_assemblies).Verifiable();
        _services.AddScoped(typeof(IUnitOfWork), _ => _uoW.Object);
        _services.AddMasaDbContext<CustomDbContext>(options => options.UseTestSqlite(Connection));
        _dispatcherOptions.Object.UseRepository<CustomDbContext>().UseRepository<CustomDbContext>();

        var serviceProvider = _services.BuildServiceProvider();
        var repository = serviceProvider.GetServices<IRepository<OrderItem>>();
        Assert.IsTrue(repository.Count() == 1);
    }
}
