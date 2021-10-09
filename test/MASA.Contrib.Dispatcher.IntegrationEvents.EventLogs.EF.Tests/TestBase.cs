using Microsoft.Data.Sqlite;

namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF.Tests;

public class TestBase
{
    private readonly SqliteConnection _connection;

    protected TestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
    }

    protected IServiceProvider CreateDefaultProvider()
    {
        var services = new ServiceCollection();
        services.AddMasaDbContext<IntegrationEventLogContext>(options => options.UseSqlite(_connection));
        services.AddScoped<IIntegrationEventLogService, IntegrationEventLogService>();
        return services.BuildServiceProvider();
    }
}
