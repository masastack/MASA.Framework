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
                builder.Prefix = "";
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

    [TestMethod]
    public void TestDefaultCaller()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller();
        var serviceProvider = services.BuildServiceProvider();
        var defaultCaller = serviceProvider.GetService<ICaller>();
        Assert.IsNotNull(defaultCaller);

        services.AddCaller(opt =>
        {
            opt.UseHttpClient(builder =>
            {
                builder.BaseAddress = "https://github.com/masastack";
            });
        });
        serviceProvider = services.BuildServiceProvider();
        defaultCaller = serviceProvider.GetRequiredService<ICaller>();
        Assert.IsNotNull(defaultCaller);
        Assert.IsTrue(defaultCaller.GetType() == typeof(HttpClientCaller));

        var httpClient = typeof(HttpClientCaller).GetField("_httpClient", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(httpClient);

        var httpClientValue = httpClient.GetValue(defaultCaller);
        Assert.IsTrue(httpClientValue != null && ((System.Net.Http.HttpClient)httpClientValue).BaseAddress != null &&
            ((System.Net.Http.HttpClient)httpClientValue).BaseAddress!.ToString() == "https://github.com/masastack");
    }

    [TestMethod]
    public async Task TestConfigHttpRequestMessageAsync()
    {
        var services = new ServiceCollection();
        services.AddScoped<TokenProvider>();
        services.AddCaller();
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        var customHeaderCaller = scope.ServiceProvider.GetService<CustomHeaderCaller>();
        Assert.IsNotNull(customHeaderCaller);
        var tokenProvider = scope.ServiceProvider.GetService<TokenProvider>();
        Assert.IsNotNull(tokenProvider);
        tokenProvider.Token = "token";
        await customHeaderCaller.GetAsync();
    }

    [TestMethod]
    public void TestDisableAutoRegistration()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddCaller(option =>
        {
            option.DisableAutoRegistration = true;
            option.UseHttpClient(clientBuilder =>
            {
                clientBuilder.BaseAddress = "https://github.com/masastack/MASA.Contrib";
            });
        });
        var serviceProvider = services.BuildServiceProvider();
        var callerFactoryOptions = serviceProvider.GetService<IOptions<CallerFactoryOptions>>();
        Assert.IsNotNull(callerFactoryOptions);
        Assert.AreEqual(1, callerFactoryOptions.Value.Options.Count);
    }
}
