namespace MASA.Contrib.Dispatcher.InMemory.Tests;

[TestClass]
public class TestBase
{
    protected IServiceProvider _serviceProvider { get; private set; }

    protected IServiceCollection _services { get; private set; }

    public TestBase() : this(null)
    {

    }

    public TestBase(Func<IServiceCollection, IServiceCollection> func = null) => this.ResetMemoryEventBus(func, false, false, null);

    protected void ResetMemoryEventBus(Func<IServiceCollection, IServiceCollection> func = null, bool isReset = true, bool isAddLog = true, params Assembly[] assemblies)
    {
        _services = new ServiceCollection();
        if (isAddLog)
        {
            _services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());
        }
        else
        {
            _services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.Services.RemoveAll(typeof(ILogger<>));
            });
        }

        _services.AddTransient(typeof(IMiddleware<>), typeof(LoggingMiddleware<>));
        func?.Invoke(_services);
        _services = assemblies == null ? _services.AddTestEventBus(ServiceLifetime.Scoped, options => options.Assemblies = _defaultAssemblies) : _services.AddTestEventBus(ServiceLifetime.Scoped, options => options.Assemblies = assemblies);
        _serviceProvider = _services.BuildServiceProvider();
    }

    private static Assembly[] _defaultAssemblies => new Assembly[1] { typeof(TestBase).Assembly };

    protected void ResetMemoryEventBus(params Assembly[] assemblies) => this.ResetMemoryEventBus(null, true, true, assemblies);

    protected void ResetMemoryEventBus(bool isAddLog, params Assembly[] assemblies) => this.ResetMemoryEventBus(null, true, isAddLog, assemblies);
}
