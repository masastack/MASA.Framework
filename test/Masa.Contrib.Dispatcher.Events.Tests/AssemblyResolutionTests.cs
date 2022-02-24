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
        services.AddTestEventBus(ServiceLifetime.Scoped, options => options.Assemblies = AppDomain.CurrentDomain.GetAssemblies());
    }

    [TestMethod]
    public void TestAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            services.AddEventBus(options => options.Assemblies = null!);
        });
    }

    [TestMethod]
    public void TestAddEmptyAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            services.AddEventBus(options => options.Assemblies = new Assembly[0]);
        });
    }

    [TestMethod]
    public void TestEventBusByAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            services.AddTestEventBus(ServiceLifetime.Scoped, options => options.Assemblies = null!);
        });
    }

    [TestMethod]
    public void TestEventBusByAddEmptyAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            services.AddTestEventBus(ServiceLifetime.Scoped, options => options.Assemblies = new Assembly[0]);
        });
    }

    [TestMethod]
    public void TestEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        services.AddTestEventBus(ServiceLifetime.Scoped);
    }

    [TestMethod]
    public void TestUseEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        var options = new DispatcherOptions(services);
        options.UseEventBus();

        var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        Assert.IsNotNull(eventBus);
    }

    [TestMethod]
    public void TestAddMultEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        var options = new DispatcherOptions(services);
        options.UseEventBus().UseEventBus();

        Assert.IsTrue(services.BuildServiceProvider().GetServices<IEventBus>().Count() == 1);

        var services2 = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services2.AddTestEventBus(ServiceLifetime.Scoped)
                 .AddTestEventBus(ServiceLifetime.Scoped);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsTrue(serviceProvider.GetServices<IEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestUseEventBusAndNullServices()
    {
        var options = new DispatcherOptions(null!);
        Assert.ThrowsException<ArgumentNullException>(() => options.UseEventBus());
    }
}
