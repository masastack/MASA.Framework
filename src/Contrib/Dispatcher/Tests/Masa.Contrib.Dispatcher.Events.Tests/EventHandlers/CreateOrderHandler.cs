// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

#pragma warning disable CA1822
public class CreateOrderHandler
{
    [EventHandler(FailureLevels = FailureLevels.ThrowAndCancel)]
    public async Task CreateOrderAsync(OrderDbContext dbContext, IUnitOfWork unitOfWork, CreateOrderEvent @event)
    {
        var order = new Order()
        {
            Id = @event.Id,
            Name = @event.Name
        };
        await dbContext.Set<Order>().AddAsync(order);
        unitOfWork.EntityState = BuildingBlocks.Data.UoW.EntityState.Changed;
        unitOfWork.CommitState = CommitState.UnCommited;
    }

    [EventHandler(IsCancel = true)]
    public Task CancelOrderAsync(CreateOrderEvent @event)
    {
        @event.IsExecuteCancel = true;
        return Task.CompletedTask;
    }
}
#pragma warning restore CA1822
