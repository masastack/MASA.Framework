// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Core.Tests;

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
                clientBuilder.Name = "http";
                clientBuilder.IsDefault = true;
                clientBuilder.BaseAddress = "https://github.com/masastack/MASA.Contrib";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerProvider1 = serviceProvider.GetRequiredService<ICallerProvider>();
        var callerProvider2 = serviceProvider.GetRequiredService<ICallerProvider>();
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
                clientBuilder.Name = "http";
                clientBuilder.IsDefault = true;
                clientBuilder.BaseAddress = "https://github.com/masastack/MASA.Contrib";
            });
            opt.UseDapr(clientBuilder =>
            {
                clientBuilder.Name = "dapr";
                clientBuilder.IsDefault = false;
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerProvider = serviceProvider.GetRequiredService<ICallerProvider>();
        Assert.IsNotNull(callerProvider);

        var caller = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient();
        var daprCaller = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient("dapr");
        var httpCaller = serviceProvider.GetRequiredService<ICallerFactory>().CreateClient("http");

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
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "gitee";
                    builder.BaseAddress = "https://gitee.com/masastack";
                    builder.IsDefault = true;
                });
            });
        });
    }

    [TestMethod]
    public void TestMultiDefaultCaller2()
    {
        IServiceCollection services = new ServiceCollection();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "gitee";
                    builder.BaseAddress = "https://gitee.com/masastack";
                    builder.IsDefault = true;
                });
            });
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
            });
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
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
            });
        });
    }

    [TestMethod]
    public void TestRepeatCallerName2()
    {
        IServiceCollection services = new ServiceCollection();
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
            });

            services.AddCaller(opt =>
            {
                opt.UseHttpClient(builder =>
                {
                    builder.Name = "github";
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
                });
            });
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
                opt.UseHttpClient(builder =>
                {
                    builder.Name = typeof(GithubCaller).FullName!;
                    builder.BaseAddress = "https://github.com/masastack";
                    builder.IsDefault = true;
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
                builder.Name = "masastack";
                builder.BaseAddress = "https://github.com/masastack";
                builder.IsDefault = true;
            });
        });
        services.AddCaller(opt =>
        {
            opt.UseHttpClient(builder =>
            {
                builder.Name = "masastack2";
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetRequiredService<ICallerFactory>().CreateClient("masastack"));
        Assert.IsNotNull(serviceProvider.GetRequiredService<ICallerFactory>().CreateClient("masastack2"));
    }
}
