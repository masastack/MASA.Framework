namespace Masa.Contrib.Dispatcher.Events.Tests;

[TestClass]
public class TestBase
{
    protected IServiceProvider _serviceProvider { get; private set; }

    protected IServiceCollection _services { get; private set; }

    public TestBase() : this(null)
    {

    }

    public TestBase(Func<IServiceCollection, IServiceCollection>? func = null) => ResetMemoryEventBus(func, false, null);

    protected void ResetMemoryEventBus(Func<IServiceCollection, IServiceCollection>? func = null, bool isAddLog = true, params Assembly[]? assemblies)
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
        _services = assemblies == null ? _services.AddTestEventBus(_defaultAssemblies, ServiceLifetime.Scoped) : _services.AddTestEventBus(assemblies, ServiceLifetime.Scoped);
        _serviceProvider = _services.BuildServiceProvider();
    }

    private static Assembly[] _defaultAssemblies => new Assembly[1] { typeof(TestBase).Assembly };

    protected void ResetMemoryEventBus(params Assembly[] assemblies) => ResetMemoryEventBus(null, true, assemblies);

    protected void ResetMemoryEventBus(bool isAddLog, params Assembly[] assemblies) => ResetMemoryEventBus(null, true, assemblies);
}
