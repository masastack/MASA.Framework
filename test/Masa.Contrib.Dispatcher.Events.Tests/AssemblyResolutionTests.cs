namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class AssemblyResolutionTests
{
    [TestMethod]
    public void TestResolveEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        services.AddEventBus();
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetService<IEventBus>();
        Assert.IsNotNull(eventBus, "Event bus injection failed");
        Assert.IsNotNull(eventBus.GetAllEventTypes());
    }

    [TestMethod]
    public void TestAddDefaultAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        services.AddTestEventBus(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Scoped);
    }

    [TestMethod]
    public void TestAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            Assembly[] assemblies = null;
            services.AddEventBus(assemblies!);
        });
    }

    [TestMethod]
    public void TestAddEmptyAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddEventBus(Array.Empty<Assembly>());
        });
    }

    [TestMethod]
    public void TestEventBusByAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddTestEventBus(null, ServiceLifetime.Scoped);
        });
    }

    [TestMethod]
    public void TestEventBusByAddEmptyAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddTestEventBus(Array.Empty<Assembly>(), ServiceLifetime.Scoped);
        });
    }

    [TestMethod]
    public void TestEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        services.AddTestEventBus(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Scoped);
    }

    [TestMethod]
    public void TestUseEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        var options = new DispatcherOptions(services, AppDomain.CurrentDomain.GetAssemblies());
        options.UseEventBus();

        var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        Assert.IsNotNull(eventBus);
    }

    [TestMethod]
    public void TestAddMultEventBus()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        var options = new DispatcherOptions(services, assemblies);
        options.UseEventBus().UseEventBus();

        Assert.IsTrue(services.BuildServiceProvider().GetServices<IEventBus>().Count() == 1);

        var services2 = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services2.AddTestEventBus(assemblies, ServiceLifetime.Scoped)
            .AddTestEventBus(assemblies, ServiceLifetime.Scoped);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestUseEventBusAndNullServices()
    {
        var options = new DispatcherOptions(null!, AppDomain.CurrentDomain.GetAssemblies());
        Assert.ThrowsException<ArgumentNullException>(() => options.UseEventBus());
    }
}
