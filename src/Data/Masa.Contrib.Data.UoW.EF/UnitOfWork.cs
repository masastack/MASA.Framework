// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EF;

public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : MasaDbContext, IMasaDbContext
{
    public IServiceProvider ServiceProvider { get; }

    private readonly DbContext? _context = null;

    protected DbContext Context => _context ?? ServiceProvider.GetRequiredService<TDbContext>();

    public DbTransaction Transaction
    {
        get
        {
            if (!UseTransaction)
                throw new NotSupportedException("Doesn't support transaction opening");

            if (TransactionHasBegun)
                return Context.Database.CurrentTransaction!.GetDbTransaction();

            return Context.Database.BeginTransaction().GetDbTransaction();
        }
    }

    public bool TransactionHasBegun => Context.Database.CurrentTransaction != null;

    public bool DisableRollbackOnFailure { get; set; }

    public EntityState EntityState { get; set; } = EntityState.UnChanged;

    public CommitState CommitState { get; set; } = CommitState.Commited;

    public bool UseTransaction { get; set; } = true;

    public UnitOfWork(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (EntityState == EntityState.UnChanged)
            return;

        await Context.SaveChangesAsync(cancellationToken);
        EntityState = EntityState.UnChanged;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        var domainEventBus = GetDomainEventBus();
        while (domainEventBus != null && await domainEventBus.AnyQueueAsync())
        {
            await domainEventBus.PublishQueueAsync();

            await SaveChangesAsync(cancellationToken);
        }

        if (UseTransaction && TransactionHasBegun && CommitState == CommitState.UnCommited)
        {
            await Context.Database.CommitTransactionAsync(cancellationToken);
            CommitState = CommitState.Commited;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (!UseTransaction || !TransactionHasBegun)
            return;

        if (TransactionHasBegun)
            await Context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public Task AddDomainEventAsync<TDomainEvent>(TDomainEvent @event) where TDomainEvent : class
    {
        var domainEventBus = GetDomainEventBus();
        if (domainEventBus == null || @event is not IDomainEvent domainEvent)
            return Task.CompletedTask;

        domainEventBus.Enqueue(domainEvent);
        return Task.CompletedTask;
    }

    public async ValueTask DisposeAsync() => await (_context?.DisposeAsync() ?? ValueTask.CompletedTask);

    public void Dispose() => _context?.Dispose();

    private IDomainEventBus? GetDomainEventBus()
        => ServiceProvider.GetService<IDomainEventBus>();
}
