// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

#pragma warning disable CS0618
namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests;

[TestClass]
public class CallerTest
{
    [TestMethod]
    public void TestCallerProviderServiceLifetime()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(clientBuilder =>
            {
                clientBuilder.BaseApi = "https://github.com/masastack/MASA.Contrib";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerProvider1 = serviceProvider.GetRequiredService<ICaller>();
        var callerProvider2 = serviceProvider.GetRequiredService<ICaller>();
        Assert.IsTrue(callerProvider1 == callerProvider2);
    }

    [TestMethod]
    public void TestCaller()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(clientBuilder =>
            {
                clientBuilder.BaseAddress = "https://github.com/masastack/MASA.Contrib";
            });
            opt.UseDapr("dapr", _ =>
            {
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var caller = serviceProvider.GetRequiredService<ICaller>();
        Assert.IsNotNull(caller);

        caller = serviceProvider.GetRequiredService<ICallerFactory>().Create();
        var daprCaller = serviceProvider.GetRequiredService<ICallerFactory>().Create("dapr");
        var httpCaller = serviceProvider.GetRequiredService<ICallerFactory>().Create();

        Assert.IsTrue(caller.GetType().FullName != daprCaller.GetType().FullName);
        Assert.IsTrue(caller.GetType().FullName == httpCaller.GetType().FullName);
    }

    [TestMethod]
    public void TestMultiDefaultCaller()
    {
        IServiceCollection services = new ServiceCollection();

        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.BaseAddress = "https://github.com/masastack";
                });
                opt.UseHttpClient(builder =>
                {
                    builder.BaseAddress = "https://gitee.com/masastack";
                });
            });
        });
    }

    [TestMethod]
    public void TestMultiDefaultCaller2()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(builder =>
            {
                builder.BaseAddress = "https://gitee.com/masastack";
            });
        });
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<CallerFactoryOptions>>();
            optionsFactory.Create(Options.DefaultName);
        });
    }

    [TestMethod]
    public void TestRepeatCallerName()
    {
        IServiceCollection services = new ServiceCollection();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.BaseAddress = "https://github.com/masastack";
                });
                opt.UseHttpClient(builder =>
                {
                    builder.BaseAddress = "https://github.com/masastack";
                });
            });
        });
    }

    [TestMethod]
    public void TestRepeatCallerName2()
    {
        IServiceCollection services = new ServiceCollection();
        var callerName = "github";
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(callerName, builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });

        services.AddCaller(opt =>
        {
            opt.UseHttpClient(callerName, builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<CallerFactoryOptions>>();
            optionsFactory.Create(Options.DefaultName);
        });
    }

    [TestMethod]
    public void TestRepeatCallerName3()
    {
        IServiceCollection services = new ServiceCollection();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(typeof(GithubCaller).FullName!, builder =>
                {
                    builder.BaseAddress = "https://github.com/masastack";
                });
            });
        });
    }

    [TestMethod]
    public void TestAddMultiCaller()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        services.AddCaller(opt =>
        {
            opt.UseHttpClient("masastack2", builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<ICallerFactory>().Create());
        Assert.IsNotNull(serviceProvider.GetRequiredService<ICallerFactory>().Create("masastack2"));
    }
}
