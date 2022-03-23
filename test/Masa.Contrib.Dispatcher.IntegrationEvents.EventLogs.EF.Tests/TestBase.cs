namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

public class TestBase : IDisposable
{
    protected readonly string ConnectionString = "DataSource=:memory:";
    protected readonly SqliteConnection Connection;

    protected TestBase()
    {
        Connection = new SqliteConnection(ConnectionString);
        Connection.Open();
    }

    public void Dispose()
    {
        Connection.Close();
    }

    protected IServiceProvider CreateDefaultProvider(Action<DispatcherOptions>? action = null)
    {
        var services = new ServiceCollection();
        services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        var options = new DispatcherOptions(services);
        services.AddMasaDbContext<CustomDbContext>(builder => builder.UseSqlite(ConnectionString));
        action?.Invoke(options);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        integrationEventBus.Setup(e => e.GetAllEventTypes()).Returns(()
            => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type)));
        services.AddScoped(serviceProvider => integrationEventBus.Object);
        return services.BuildServiceProvider();
    }
}

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; }

    public Assembly[] Assemblies { get; }

    public DispatcherOptions(IServiceCollection services, Assembly[]? assemblies = null)
    {
        this.Services = services;
        Assemblies = assemblies ?? AppDomain.CurrentDomain.GetAssemblies();
    }
}
