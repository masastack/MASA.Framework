// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Ddd.Domain.Repositories;

public interface IRepository<TEntity>
    where TEntity : class, IEntity
{
    [Obsolete("UnitOfWork is not part of IRepository, it will be removed in 1.0")]
    IUnitOfWork UnitOfWork { get; }

    #region Add

    ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Update

    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    #endregion

    #region Remove

    Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Find

    Task<TEntity?> FindAsync(IEnumerable<KeyValuePair<string, object>> keyValues, CancellationToken cancellationToken = default);

    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    #endregion

    #region Get

    Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListAsync(string sortField, bool isDescending = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(CancellationToken cancellationToken = default);

    Task<long> GetCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sortField">Sort field name</param>
    /// <param name="isDescending">true descending order, false ascending order, default: true</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sorting">Key: sort field name, Value: true descending order, false ascending order</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default);

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
    Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, string sortField,
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
    Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take,
        Dictionary<string, bool>? sorting = null, CancellationToken cancellationToken = default);

    Task<PaginatedList<TEntity>> GetPaginatedListAsync(PaginatedOptions options, CancellationToken cancellationToken = default);

    Task<PaginatedList<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, PaginatedOptions options,
        CancellationToken cancellationToken = default);

    #endregion

}

public interface IRepository<TEntity, TKey> : IRepository<TEntity>
    where TEntity : class, IEntity<TKey>
    where TKey : IComparable
{

    #region Find

    Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default);

    #endregion

    #region Remove

    Task RemoveAsync(TKey id, CancellationToken cancellationToken = default);

    Task RemoveRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);

    #endregion

}

public interface IRepository<TDbContext, TEntity, TKey> : IRepository<TEntity, TKey>
    where TDbContext : IMasaDbContext
    where TEntity : class, IEntity<TKey>
    where TKey : IComparable
{

}
