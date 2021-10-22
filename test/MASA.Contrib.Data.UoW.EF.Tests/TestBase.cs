using Microsoft.Data.Sqlite;

namespace MASA.Contrib.Data.UoW.EF.Tests
{
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

        private IServiceProvider CreateDefaultProvider()
        {
            var options = new Mock<IDispatcherOptions>();
            options.Setup(option => option.Services).Returns(new ServiceCollection()).Verifiable();
            options.Object.UseUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
            return options.Object.Services.BuildServiceProvider();
        }

        protected (IServiceProvider serviceProvider, CustomerDbContext dbContext) CreateDefault()
        {
            var serviceProvider = CreateDefaultProvider();
            var dbContext = serviceProvider.GetRequiredService<CustomerDbContext>();
            dbContext.Database.EnsureCreated();
            return (serviceProvider, dbContext);
        }
    }
}
