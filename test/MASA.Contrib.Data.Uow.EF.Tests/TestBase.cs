using Microsoft.Data.Sqlite;

namespace MASA.Contrib.Data.Uow.EF.Tests
{
    public class TestBase : IDisposable
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

        protected IServiceProvider CreateProviderByEmptyDbConnectionString()
        {
            var services = new ServiceCollection();
            services.AddUoW<CustomerDbContext>();
            return services.BuildServiceProvider();
        }

        private IServiceProvider CreateDefaultProvider()
        {
            var services = new ServiceCollection();
            services.AddUoW<CustomerDbContext>(options => options.UseSqlite(_connection));
            return services.BuildServiceProvider();
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
