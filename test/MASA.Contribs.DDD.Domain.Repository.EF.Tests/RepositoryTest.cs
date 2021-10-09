namespace MASA.Contribs.DDD.Domain.Repository.EF.Tests;

[TestClass]
public class RepositoryTest : TestBase
{
    private readonly Assembly[] _assemblies;

    public RepositoryTest()
    {
        _assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    [TestMethod]
    public void TestEmptyAssembly()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var serviceProvider = base.CreateDefaultServiceProvider(new Assembly[0]);
        });
    }

    [TestMethod]
    public void TestNullAssembly()
    {
        Assert.ThrowsException<ArgumentNullException>(() =>
        {
            var serviceProvider = base.CreateDefaultServiceProvider(null!);
        });
    }

    [TestMethod]
    public void TestNoUnitOfWorkAssembly()
    {
        Assert.ThrowsException<NotImplementedException>(() =>
        {
            var serviceProvider = base.CreateServiceProvider(null, _assemblies);
        });
    }

    [TestMethod]
    public void TestBaseRepository()
    {
        var serviceProvider = base.CreateDefaultServiceProvider(_assemblies)!;
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
}
