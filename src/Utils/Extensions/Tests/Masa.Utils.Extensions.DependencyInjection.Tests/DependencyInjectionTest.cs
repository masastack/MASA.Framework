// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Extensions.DependencyInjection.Tests.Domain.Models;

namespace Masa.Utils.Extensions.DependencyInjection.Tests;

[TestClass]
public class DependencyInjectionTest
{
    private DefaultTypeProvider _typeProvider = default!;
    private IEnumerable<Type> _allTypes = default!;

    [TestInitialize]
    public void Initialize()
    {
        _typeProvider = new DefaultTypeProvider();
        _allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
    }

    [TestMethod]
    public void TestSkip()
    {
        Assert.IsFalse(_typeProvider.IsSkip(typeof(BaseService)));
        Assert.IsTrue(_typeProvider.IsSkip(typeof(GoodsBaseService)));
        Assert.IsFalse(_typeProvider.IsSkip(typeof(GoodsService)));
        Assert.IsFalse(_typeProvider.IsSkip(typeof(NullCalculateProviderService)));
        Assert.IsTrue(_typeProvider.IsSkip(typeof(OrderBaseService)));
        Assert.IsTrue(_typeProvider.IsSkip(typeof(OrderService)));
        Assert.IsTrue(_typeProvider.IsSkip(typeof(UserBaseService)));
        Assert.IsFalse(_typeProvider.IsSkip(typeof(UserService)));
    }

    [TestMethod]
    public void TestGetServiceTypesReturnCountIs5()
    {
        var serviceTypes = _typeProvider.GetServiceTypes(_allTypes.ToList(), typeof(ISingletonDependency));
        Assert.IsTrue(serviceTypes.Count == 5);
    }

    [TestMethod]
    public void TestGetImplementationTypesReturnCountIs1()
    {
        var implementationTypes = _typeProvider.GetImplementationTypes(_allTypes.ToList(), typeof(ICalculateProviderService));
        Assert.IsTrue(implementationTypes.Count == 1);

        implementationTypes = _typeProvider.GetImplementationTypes(_allTypes.ToList(), typeof(BaseService));
        Assert.IsTrue(implementationTypes.Count == 1);
    }

    [TestMethod]
    public void TestAssignableFrom()
    {
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(ICalculateProviderService), typeof(NullCalculateProviderService)));
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(ISingletonDependency), typeof(ICalculateProviderService)));

        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(NullCalculateProviderService), typeof(ICalculateProviderService)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(ICalculateProviderService), typeof(ISingletonDependency)));

        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(BaseService), typeof(UserBaseService)));
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(BaseService), typeof(UserService)));
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(UserBaseService), typeof(UserService)));

        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(UserBaseService), typeof(BaseService)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(UserService), typeof(BaseService)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(UserService), typeof(UserBaseService)));

        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(IRepository<>), typeof(RepositoryBase<>)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(IRepository<>), typeof(RepositoryBase<User>)));
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(IRepository<User>), typeof(RepositoryBase<User>)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(RepositoryBase<>), typeof(IRepository<>)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(RepositoryBase<User>), typeof(IRepository<User>)));

        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(IRepository<,>), typeof(UserRepository<>)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(IRepository<UserDbContext, User>), typeof(UserRepository<>)));
        Assert.IsFalse(_typeProvider.IsAssignableFrom(typeof(UserRepository<>), typeof(IRepository<,>)));
        Assert.IsTrue(_typeProvider.IsAssignableFrom(typeof(IRepository<UserDbContext, User>), typeof(UserRepository<User>)));
    }

    [TestMethod]
    public void TestAssignableTo()
    {
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(ICalculateProviderService), typeof(NullCalculateProviderService)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(ISingletonDependency), typeof(ICalculateProviderService)));

        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(NullCalculateProviderService), typeof(ICalculateProviderService)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(ICalculateProviderService), typeof(ISingletonDependency)));

        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(BaseService), typeof(UserBaseService)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(BaseService), typeof(UserService)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(UserBaseService), typeof(UserService)));

        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(UserBaseService), typeof(BaseService)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(UserService), typeof(BaseService)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(UserService), typeof(UserBaseService)));

        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<>), typeof(RepositoryBase<>)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<>), typeof(RepositoryBase<User>)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<User>), typeof(RepositoryBase<User>)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(RepositoryBase<>), typeof(IRepository<>)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(RepositoryBase<User>), typeof(IRepository<User>)));

        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<,>), typeof(UserRepository<>)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<UserDbContext, User>), typeof(UserRepository<>)));
        Assert.IsTrue(_typeProvider.IsAssignableTo(typeof(UserRepository<>), typeof(IRepository<,>)));
        Assert.IsFalse(_typeProvider.IsAssignableTo(typeof(IRepository<UserDbContext, User>), typeof(UserRepository<User>)));
    }

    [TestMethod]
    public void TestAddAutoInject()
    {
        var services = new ServiceCollection();
        services.AddAutoInject();
        var serviceProvider = services.BuildServiceProvider();
        var calculateProviderService = serviceProvider.GetService<ICalculateProviderService>();
        Assert.IsNotNull(calculateProviderService);
        Assert.IsNull(serviceProvider.GetService<NullCalculateProviderService>());

        Assert.IsTrue(BaseService.Count == 1);
        var serviceBase = serviceProvider.GetService<BaseService>();
        Assert.IsNotNull(serviceBase);

        var userBaseService = serviceProvider.GetService<UserBaseService>();
        Assert.IsNull(userBaseService);

        Assert.IsTrue(UserService.UserCount == 1);
        var userService = serviceProvider.GetService<UserService>();
        Assert.IsNotNull(userService);

        var goodsBaseService = serviceProvider.GetService<GoodsBaseService>();
        Assert.IsNull(goodsBaseService);

        Assert.IsTrue(GoodsService.GoodsCount == 1);
        var goodsService = serviceProvider.GetService<GoodsService>();
        Assert.IsNotNull(goodsService);

        var orderBaseService = serviceProvider.GetService<OrderBaseService>();
        Assert.IsNull(orderBaseService);

        var orderService = serviceProvider.GetService<OrderService>();
        Assert.IsNull(orderService);
    }

    [TestMethod]
    public void TestAddAutoInjectAndEmptyAssemblyReturnServiceIsNull()
    {
        var services = new ServiceCollection();
        services.AddAutoInject(Array.Empty<Assembly>());
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNull(serviceProvider.GetService<IRepository<User>>());
        Assert.IsNull(serviceProvider.GetService<BaseService>());
    }

    [TestMethod]
    public void TestAddAutoInjectMultiReturnCountIs1()
    {
        var services = new ServiceCollection();
        services
            .AddAutoInject(typeof(IRepository<>).Assembly)
            .AddAutoInject(typeof(IRepository<>).Assembly);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IRepository<User>>());
        Assert.IsTrue(serviceProvider.GetServices<IRepository<User>>().Count() == 1);
    }

    [TestMethod]
    public void TestAny()
    {
        var services = new ServiceCollection();
        Assert.IsFalse(services.Any<User>());
        Assert.IsFalse(services.Any<User>(ServiceLifetime.Singleton));
        services.AddScoped<User>();
        Assert.IsTrue(services.Any<User>());
        Assert.IsFalse(services.Any<User>(ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any<User>(ServiceLifetime.Scoped));
        Assert.IsFalse(services.Any<User>(ServiceLifetime.Transient));
    }

    [TestMethod]
    public void TestDependencyReturnProviderServiceIs1()
    {
        var services = new ServiceCollection();
        services.AddAutoInject();
        var serviceProvider = services.BuildServiceProvider();
        var factories = serviceProvider.GetServices<IClientFactory>().ToList();
        Assert.IsTrue(factories.Count == 1);

        Assert.IsTrue(factories[0].GetClientName() == nameof(CustomizeClientFactory));
    }
}
