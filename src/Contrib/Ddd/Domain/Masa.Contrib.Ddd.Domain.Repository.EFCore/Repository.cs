// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EFCore;

public class Repository<TDbContext, TEntity> :
    RepositoryBase<TEntity>
    where TEntity : class, IEntity
    where TDbContext : DbContext, IMasaDbContext
{
    protected TDbContext Context { get; }

    public Repository(TDbContext context, IUnitOfWork unitOfWork) : base(unitOfWork.ServiceProvider)
    {
        Context = context;
        UnitOfWork = unitOfWork;
    }

    public override IUnitOfWork UnitOfWork { get; }

    public override EntityState EntityState
    {
        get => UnitOfWork.EntityState;
        set
        {
            UnitOfWork.EntityState = value;
            if (value == EntityState.Changed)
                CheckAndOpenTransaction();
        }
    }

    public override async ValueTask<TEntity> AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        var response = (await Context.AddAsync(entity, cancellationToken)).Entity;
        EntityState = EntityState.Changed;
        return response;
    }

    public override async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await Context.AddRangeAsync(entities, cancellationToken);
        EntityState = EntityState.Changed;
    }

    public override Task<TEntity?> FindAsync(
        IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> fields = new(keyValues);
        return Context.Set<TEntity>().IgnoreQueryFilters().GetQueryable(fields).FirstOrDefaultAsync(cancellationToken);
    }

    public override Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().LongCountAsync(cancellationToken);

    public override Task<long> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().LongCountAsync(predicate, cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().OrderBy(sortField, isDescending).ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        string sortField,
        bool isDescending = true,
        CancellationToken cancellationToken = default)
        => await Context.Set<TEntity>().OrderBy(sortField, isDescending).Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sortField">Sort field name</param>
    /// <param name="isDescending">true descending order, false ascending order, default: true</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, string sortField, bool isDescending = true,
        CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().OrderBy(sortField, isDescending).Skip(skip).Take(take).ToListAsync(cancellationToken);

    /// <summary>
    /// Get a paginated list after sorting according to skip and take
    /// </summary>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sorting">Key: sort field name, Value: true descending order, false ascending order</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TEntity>> GetPaginatedListAsync(
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return Context.Set<TEntity>().OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

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
    public override Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take,
        string sortField,
        bool isDescending = true, CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().Where(predicate).OrderBy(sortField, isDescending).Skip(skip).Take(take).ToListAsync(cancellationToken);

    /// <summary>
    /// Get a paginated list after sorting by condition
    /// </summary>
    /// <param name="predicate"> A function to test each element for a condition</param>
    /// <param name="skip">The number of elements to skip before returning the remaining elements</param>
    /// <param name="take">The number of elements to return</param>
    /// <param name="sorting">Key: sort field name, Value: true descending order, false ascending order</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TEntity>> GetPaginatedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return Context.Set<TEntity>().Where(predicate).OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public override Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Remove(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken);
        EntityState = EntityState.Changed;
        Context.Set<TEntity>().RemoveRange(entities);
    }

    public override Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().RemoveRange(entities);
        EntityState = EntityState.Changed;
        return Task.CompletedTask;
    }

    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().Update(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        Context.Set<TEntity>().UpdateRange(entities);
        EntityState = EntityState.Changed;
        return Task.CompletedTask;
    }

    /// <summary>
    /// When additions, deletions and changes are made through the Repository and the transaction is currently allowed and the transaction is not opened, the transaction is started
    /// </summary>
    private void CheckAndOpenTransaction()
    {
        if (!UnitOfWork.UseTransaction)
            return;

        if (!UnitOfWork.TransactionHasBegun)
        {
            _ = UnitOfWork.Transaction; // Open the transaction
        }
        CommitState = CommitState.UnCommited;
    }
}

public class Repository<TDbContext, TEntity, TKey> :
    Repository<TDbContext, TEntity>,
    IRepository<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    where TDbContext : DbContext, IMasaDbContext
    where TKey : IComparable
{
    public Repository(TDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public virtual Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
        => Context.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id), cancellationToken);

    public virtual Task RemoveAsync(TKey id, CancellationToken cancellationToken = default)
        => base.RemoveAsync(entity => entity.Id.Equals(id), cancellationToken);

    public virtual Task RemoveRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
        => base.RemoveAsync(entity => ids.Contains(entity.Id), cancellationToken);
}
