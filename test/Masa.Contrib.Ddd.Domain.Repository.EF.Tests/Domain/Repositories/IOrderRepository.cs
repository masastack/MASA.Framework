namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Domain.Repositories;

public interface IOrderRepository : IRepository<Orders>
{
    Task AddAsync(Orders order);
}

public interface ICustomizeOrderRepository : IRepository<Orders, int>
{

}
