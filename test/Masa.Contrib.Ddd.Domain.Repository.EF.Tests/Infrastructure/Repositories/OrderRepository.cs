namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Infrastructure.Repositories;

public class OrderRepository : Repository<CustomDbContext, Orders>, IOrderRepository
{
    public OrderRepository(CustomDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public async Task AddAsync(Orders order)
    {
        try
        {
            var transaction = base.Transaction;
            await base.AddAsync(order, default);
            await base.SaveChangesAsync();
            await base.CommitAsync();
        }
        catch (Exception)
        {
            await base.RollbackAsync();
        }
    }
}
