// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

[Obsolete("BaseRepository has expired, please use RepositoryBase")]
public abstract class BaseRepository<TEntity> :
    RepositoryBase<TEntity>
    where TEntity : class, IEntity
{
    protected BaseRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}

public abstract class RepositoryBase<TEntity> :
    IRepository<TEntity>
    where TEntity : class, IEntity
{
    public IServiceProvider ServiceProvider { get; }

    public virtual EntityState EntityState
    {
        get => UnitOfWork.EntityState;
        set => UnitOfWork.EntityState = value;
    }

    public virtual CommitState CommitState
    {
        get => UnitOfWork.CommitState;
        protected set => UnitOfWork.CommitState = value;
    }

    protected RepositoryBase(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    public abstract IUnitOfWork UnitOfWork { get; }

    #region IRepository<TEntity>

    public abstract ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await AddAsync(entity, cancellationToken);
        }
    }

    public abstract Task<TEntity?> FindAsync(IEnumerable<KeyValuePair<string, object>> keyValues, CancellationToken cancellationToken = default);

    public abstract Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public abstract Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    public abstract Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public virtual async Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await RemoveAsync(entity, cancellationToken);
        }
    }

    public abstract Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        foreach (var entity in entities)
        {
            await UpdateAsync(entity, cancellationToken);
        }
    }

    public abstract Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TEntity>> GetListAsync(string sortField, bool isDescending = true, CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    public abstract Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, string sortField, bool isDescending = true, CancellationToken cancellationToken = default);

    public abstract Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    public abstract Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sortField">Sort field name</param>
    /// <param name="isDescending">true descending order, false ascending order, default: true</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, string sortField, bool isDescending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sorting">Key: sort field name, Value: true descending order, false ascending order</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, Dictionary<string, bool>? sorting = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting by condition
    /// </summary>
    /// <param name="predicate"> A function to test each element for a condition</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sortField">Sort field name</param>
    /// <param name="isDescending">true descending order, false ascending order, default: true</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, string sortField,
        bool isDescending = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting by condition
    /// </summary>
    /// <param name="predicate"> A function to test each element for a condition</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sorting">Key: sort field name, Value: true descending order, false ascending order</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, Dictionary<string, bool>? sorting = null, CancellationToken cancellationToken = default);

    public virtual async Task<PaginatedList<TEntity>> GetPaginatedListAsync(PaginatedOptions options, CancellationToken cancellationToken = default)
    {
        var result = await GetPaginatedListAsync(
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken
        );

        var total = await GetCountAsync(cancellationToken);

        return new PaginatedList<TEntity>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    public async Task<PaginatedList<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, PaginatedOptions options, CancellationToken cancellationToken = default)
    {
        var result = await GetPaginatedListAsync(
            predicate,
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken
        );

        var total = await GetCountAsync(predicate, cancellationToken);

        return new PaginatedList<TEntity>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    #endregion
}
