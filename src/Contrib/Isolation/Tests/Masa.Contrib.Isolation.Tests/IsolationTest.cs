// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Isolation.Tests;

[TestClass]
public class IsolationTest
{
    // [TestMethod]
    // public void TestGetDbContextOptionsList()
    // {
    //     var services = new ServiceCollection();
    //     services.Configure<IsolationDbConnectionOptions>(option =>
    //     {
    //         option.ConnectionStrings = new ConnectionStrings()
    //         {
    //             DefaultConnection = "data source=test2"
    //         };
    //         option.IsolationConnectionStrings = new()
    //         {
    //             new()
    //             {
    //                 Environment = "dev",
    //                 ConnectionString = "data source=test3"
    //             },
    //             new()
    //             {
    //                 Environment = "pro",
    //                 ConnectionString = "data source=test4"
    //             }
    //         };
    //     });
    //     services.AddSingleton<IDbConnectionStringProvider, IsolationDbContextProvider>();
    //     var serviceProvider = services.BuildServiceProvider();
    //     var provider = serviceProvider.GetRequiredService<IDbConnectionStringProvider>();
    //     Assert.IsTrue(provider.DbContextOptionsList.Distinct().Count() == 3);
    // }
    //
    // [TestMethod]
    // public void TestUseIsolation()
    // {
    //     var services = new ServiceCollection();
    //     Mock<IEventBusBuilder> eventBuilder = new();
    //     eventBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
    //     Assert.ThrowsException<NotSupportedException>(() =>
    //     {
    //         eventBuilder.Object.UseIsolation(isolationBuilder =>
    //         {
    //         });
    //     }, "Tenant isolation and environment isolation use at least one");
    // }

    // [TestMethod]
    // public void TestUseIsolation2()
    // {
    //     IServiceCollection services = null!;
    //     Mock<IEventBusBuilder> eventBuilder = new();
    //     eventBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
    //     Assert.ThrowsException<ArgumentNullException>(() =>
    //     {
    //         eventBuilder.Object.UseIsolation(isolationBuilder =>
    //         {
    //         });
    //     });
    // }

    // [TestMethod]
    // public void TestUseIsolation3()
    // {
    //     IServiceCollection services = null!;
    //     Mock<IDispatcherOptions> options = new();
    //     options.Setup(option => option.Services).Returns(services).Verifiable();
    //     Assert.ThrowsException<ArgumentNullException>(() =>
    //     {
    //         options.Object.UseIsolation(isolationBuilder =>
    //         {
    //         });
    //     });
    // }

    // [TestMethod]
    // public void TestUseIsolation4()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     Mock<IEventBusBuilder> eventBuilder = new();
    //     eventBuilder.Setup(builder => builder.Services).Returns(services).Verifiable();
    //     Assert.ThrowsException<ArgumentNullException>(() =>
    //     {
    //         eventBuilder.Object.UseIsolation(null!);
    //     });
    // }

    // [TestMethod]
    // public void TestUseIsolation5()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     Mock<IDispatcherOptions> options = new();
    //     options.Setup(option => option.Services).Returns(services).Verifiable();
    //     Assert.ThrowsException<ArgumentNullException>(() =>
    //     {
    //         options.Object.UseIsolation(null!);
    //     });
    // }
    //
    // [TestMethod]
    // public void TestUseIsolation6()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     Mock<IDispatcherOptions> options = new();
    //     options.Setup(option => option.Services).Returns(services).Verifiable();
    //     options.Object.UseIsolation(isolationBuilder => isolationBuilder.UseMultiEnvironment());
    //     var serviceProvider = services.BuildServiceProvider();
    //     Assert.IsTrue(services.Count(service => service.ServiceType == typeof(IIsolationMiddleware)) == 1);
    //
    //     Assert.IsTrue(serviceProvider.GetServices<IMultiEnvironmentContext>().Count() == 1);
    //     Assert.IsTrue(!serviceProvider.GetServices<IMultiTenantContext>().Any());
    // }
    //
    // [TestMethod]
    // public void TestUseIsolation7()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     Mock<IDispatcherOptions> options = new();
    //     options.Setup(option => option.Services).Returns(services).Verifiable();
    //
    //     options.Object.UseIsolation(isolationBuilder => isolationBuilder.UseMultiTenant());
    //
    //     var serviceProvider = services.BuildServiceProvider();
    //     Assert.IsTrue(services.Count(service => service.ServiceType == typeof(IIsolationMiddleware)) == 1);
    //
    //     Assert.IsTrue(!serviceProvider.GetServices<IMultiEnvironmentContext>().Any());
    //     Assert.IsTrue(serviceProvider.GetServices<IMultiTenantContext>().Count() == 1);
    // }
    //
    // [TestMethod]
    // public void TestUseIsolation8()
    // {
    //     IServiceCollection services = new ServiceCollection();
    //     Mock<IDispatcherOptions> options = new();
    //     options.Setup(option => option.Services).Returns(services).Verifiable();
    //     options.Object.UseIsolation(isolationBuilder
    //         => isolationBuilder.UseMultiTenant().UseMultiEnvironment());
    //     options.Object.UseIsolation(isolationBuilder
    //         => isolationBuilder.UseMultiTenant().UseMultiEnvironment());
    //
    //     var serviceProvider = services.BuildServiceProvider();
    //     Assert.IsTrue(services.Count(service => service.ServiceType == typeof(IIsolationMiddleware)) == 2);
    //
    //     Assert.IsTrue(serviceProvider.GetServices<IMultiEnvironmentContext>().Count() == 1);
    //     Assert.IsTrue(serviceProvider.GetServices<IMultiTenantContext>().Count() == 1);
    // }
}
