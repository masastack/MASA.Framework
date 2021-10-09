namespace MASA.Contribs.DDD.Domain.Repository.EF.Tests.Infrastructure.Repositories;

public class OrderRepository : Repository<OrderDbContext, Orders>, IOrderRepository
{
    public OrderRepository(OrderDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
