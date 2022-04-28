// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

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
            if (UnitOfWork.UseTransaction)
                _ = base.Transaction;

            await base.AddAsync(order, default);
            await base.SaveChangesAsync();
            await base.CommitAsync();
        }
        catch (Exception)
        {
            if (UnitOfWork.UseTransaction)
                await base.RollbackAsync();
        }
    }
}

public class CustomizeOrderRepository : Repository<CustomDbContext, Orders, int>, ICustomizeOrderRepository
{
    public CustomizeOrderRepository(CustomDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }
}
