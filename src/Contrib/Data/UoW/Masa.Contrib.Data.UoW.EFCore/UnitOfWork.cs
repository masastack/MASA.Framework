// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.UoW.EFCore;

public class UnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DefaultMasaDbContext, IMasaDbContext
{
    public IServiceProvider ServiceProvider { get; }

    private DbContext? _context;

    protected DbContext Context => _context ??= ServiceProvider.GetRequiredService<TDbContext>();

    public DbTransaction Transaction
    {
        get
        {
            if (UseTransaction is false)
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

    public bool? UseTransaction { get; set; } = null;

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
        var domainEventBus = ServiceProvider.GetService<IDomainEventBus>();
        while (domainEventBus != null && await domainEventBus.AnyQueueAsync())
        {
            await domainEventBus.PublishQueueAsync();

            await SaveChangesAsync(cancellationToken);
        }

        if (UseTransaction is not false && TransactionHasBegun && CommitState == CommitState.UnCommited)
        {
            await Context.Database.CommitTransactionAsync(cancellationToken);
            CommitState = CommitState.Commited;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (UseTransaction is false || !TransactionHasBegun)
            return;

        if (TransactionHasBegun)
        {
            DetachAll();
            await Context.Database.RollbackTransactionAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Release entity tracking, prevent pre-submit exceptions, be processed by other Handlers and continue execution
    /// </summary>
    private void DetachAll()
    {
        var entityEntries = Context.ChangeTracker.Entries();
        foreach (var entry in entityEntries)
        {
            if (entry != null)
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        DisposeAsync(true);
        await (_context?.DisposeAsync() ?? ValueTask.CompletedTask);
        GC.SuppressFinalize(this);
    }

    protected virtual void DisposeAsync(bool disposing)
    {
    }

    public void Dispose()
    {
        Dispose(true);
        _context?.Dispose();
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
}
