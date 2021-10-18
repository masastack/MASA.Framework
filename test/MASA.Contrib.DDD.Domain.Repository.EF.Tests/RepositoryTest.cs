namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests;

[TestClass]
public class RepositoryTest : TestBase
{
    private readonly Assembly[] _assemblies;

    public RepositoryTest()
    {
        _assemblies = new Assembly[1]
        {
            typeof(RepositoryTest).Assembly
        };
    }

    [TestMethod]
    public void TestNoServices()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var options = new DispatcherOptions(null).UseRepository<OrderDbContext>();
        });
    }

    [TestMethod]
    public void TestUseCustomRepositoryAndNotImplementation()
    {
        var services = new ServiceCollection();

        Mock<IUnitOfWork> uow = new();
        services.AddScoped(serviceProvider => uow.Object);

        Assert.ThrowsException<NotSupportedException>(() => new DispatcherOptions(services).UseRepository<OrderDbContext>(typeof(TestBase).Assembly, typeof(IUserRepository).Assembly));
    }

    [TestMethod]
    public void TestNoUnitOfWorkAssembly()
    {
        Assert.ThrowsException<Exception>(() =>
        {
            var serviceProvider = base.CreateServiceProvider(null, _assemblies);
        });
    }

    [TestMethod]
    public void TestNullAssembly()
    {
        var serviceProvider = base.CreateDefaultServiceProvider(null)!;
        var repository = serviceProvider.GetRequiredService<IRepository<Orders>>();
        Assert.IsNotNull(repository);
        repository.AddAsync(new Orders()
        {
            BuyerName = "lisa"
        });
    }

    [TestMethod]
    public void TestCustomRepository()
    {
        var serviceProvider = base.CreateDefaultServiceProvider(_assemblies)!;
        IOrderRepository orderRepository = serviceProvider.GetRequiredService<IOrderRepository>();
        Assert.IsNotNull(orderRepository);
        orderRepository.AddAsync(new Orders()
        {
            BuyerName = "lisa"
        });
    }

    [TestMethod]
    public void TestAddMultRepository()
    {
        var services = new ServiceCollection();
        Mock<IUnitOfWork> unitOfWork = new();
        services.AddScoped(typeof(IUnitOfWork), serviceProvider => unitOfWork.Object);
        services.AddDbContext<OrderDbContext>(options => options.UseSqlite(_connection));
        new DispatcherOptions(services).UseRepository<OrderDbContext>(_assemblies).UseRepository<OrderDbContext>(_assemblies);

        var serviceProvider = services.BuildServiceProvider();
        var repository = serviceProvider.GetServices<IRepository<Orders>>();
        Assert.IsTrue(repository.Count() == 1);
    }
}
