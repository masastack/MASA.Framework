namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests;

public class TestBase : IDisposable
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
}
