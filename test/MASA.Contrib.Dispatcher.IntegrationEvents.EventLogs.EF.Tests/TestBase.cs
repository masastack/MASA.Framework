using MASA.BuildingBlocks.Dispatcher.Events;
using Microsoft.Data.Sqlite;
using Moq;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

public class TestBase
{
    protected readonly SqliteConnection _connection;

    protected TestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
    }

    protected IServiceProvider CreateDefaultProvider(Action<DispatcherOptions>? action = null)
    {
        var services = new ServiceCollection();
        services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        var options = new DispatcherOptions(services);
        options.UseEventLog(options => options.UseSqlite(_connection));
        action?.Invoke(options);

        var integrationEventBus = new Mock<IIntegrationEventBus>();
        integrationEventBus.Setup(e => e.GetAllEventTypes()).Returns(() => AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type)));
        services.AddScoped(serviceProvider => integrationEventBus.Object);
        return services.BuildServiceProvider();
    }
}

public class DispatcherOptions : IDispatcherOptions
{
    public IServiceCollection Services { get; init; }

    public DispatcherOptions(IServiceCollection services)
    {
        this.Services = services;
    }
}
