// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.UoW;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IServiceProvider ServiceProvider { get; }

    DbTransaction Transaction { get; }

    /// <summary>
    /// Whether the transaction has been opened
    /// </summary>
    bool TransactionHasBegun { get; }

    /// <summary>
    /// Whether to use transaction
    /// </summary>
    bool UseTransaction { get; set; }

    /// <summary>
    /// Disable transaction rollback after failure
    /// </summary>
    bool DisableRollbackOnFailure { get; set; }

    EntityState EntityState { get; set; }

    CommitState CommitState { get; set; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
