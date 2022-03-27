namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class AssemblyResolutionTests
{
    [TestMethod]
    public void TestResolveEventBus()
    {
        var services = new ServiceCollection();
        services
            .AddEventBus(eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(LoggingMiddleware<>)))
            .AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetService<IEventBus>();
        Assert.IsNotNull(eventBus, "Event bus injection failed");
        Assert.IsNotNull(eventBus.GetAllEventTypes());
    }

    [TestMethod]
    public void TestAddNullAssembly()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
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
        Assert.ThrowsException<ArgumentException>(() =>
        {
            services.AddTestEventBus(null, ServiceLifetime.Scoped);
        });
    }

    // [TestMethod]
    // public void TestEventBusByAddEmptyAssembly()
    // {
    //     var services = new ServiceCollection();
    //     services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
    //     services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
    //     Assert.ThrowsException<ArgumentException>(() =>
    //     {
    //         services.AddTestEventBus(Array.Empty<Assembly>(), ServiceLifetime.Scoped);
    //     });
    // }

    [TestMethod]
    public void TestEventBus()
    {
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        services.AddTestEventBus(AppDomain.CurrentDomain.GetAssemblies(), ServiceLifetime.Scoped,eventBusBuilder => eventBusBuilder.UseMiddleware(typeof(LoggingMiddleware<>)));
        var serviceProvider = services.BuildServiceProvider();
        var eventBus = serviceProvider.GetService<IEventBus>();
        Assert.IsNotNull(eventBus, "Event bus injection failed");
        Assert.IsNotNull(eventBus.GetAllEventTypes());
    }

    [TestMethod]
    public void TestUseEventBus()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        dispatcherOptions.Object.UseEventBus(eventBuilder => eventBuilder.UseMiddleware(typeof(LoggingMiddleware<>)));
        var eventBus = services.BuildServiceProvider().GetService<IEventBus>();
        Assert.IsNotNull(eventBus);
    }

    [TestMethod]
    public void TestAddMultEventBus()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var services = new ServiceCollection();
        services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        dispatcherOptions.Object
            .UseEventBus()
            .UseEventBus();

        Assert.IsTrue(services.BuildServiceProvider().GetServices<IEventBus>().Count() == 1);
    }

    [TestMethod]
    public void TestUseEventBusAndNullServices()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        IServiceCollection services = null!;
        Mock<IDispatcherOptions> dispatcherOptions = new();
        dispatcherOptions.Setup(option => option.Assemblies).Returns(assemblies).Verifiable();
        dispatcherOptions.Setup(option => option.Services).Returns(services).Verifiable();
        Assert.ThrowsException<ArgumentNullException>(() => dispatcherOptions.Object.UseEventBus());
    }
}
