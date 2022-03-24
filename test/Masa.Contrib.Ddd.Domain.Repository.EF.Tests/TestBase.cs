namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests;

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
}
