namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests;

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


    protected IServiceProvider CreateDefaultServiceProvider(params Assembly[] assemblies)
    {
        return CreateServiceProvider(services =>
        {
            Mock<IUnitOfWork> unitOfWork = new();
            services.AddScoped(typeof(IUnitOfWork), serviceProvider => unitOfWork.Object);
            services.AddDbContext<OrderDbContext>(options => options.UseSqlite(_connection));
        }, assemblies);
    }

    protected IServiceProvider CreateServiceProvider(Action<IServiceCollection>? action, params Assembly[] assemblies)
    {
        var services = new ServiceCollection();
        action?.Invoke(services);

        new DispatcherOptions(services).UseRepository<OrderDbContext>(assemblies);
        return services.BuildServiceProvider();
    }
}
