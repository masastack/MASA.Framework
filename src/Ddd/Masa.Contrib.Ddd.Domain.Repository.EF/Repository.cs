namespace Masa.Contrib.Ddd.Domain.Repository.EF;

public class Repository<TDbContext, TEntity> :
    BaseRepository<TEntity>
    where TEntity : class, IEntity
    where TDbContext : DbContext
{
    protected readonly TDbContext _context;

    public Repository(TDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        UnitOfWork = unitOfWork;
    }

    public override bool TransactionHasBegun
        => _context.Database.CurrentTransaction != null;

    public override DbTransaction Transaction
    {
        get
        {
            if (!UseTransaction)
                throw new NotSupportedException(nameof(Transaction));

            if (TransactionHasBegun)
                return _context.Database.CurrentTransaction!.GetDbTransaction();

            return _context.Database.BeginTransaction().GetDbTransaction();
        }
    }

    public override bool UseTransaction
    {
        get => UnitOfWork.UseTransaction;
        set => UnitOfWork.UseTransaction = value;
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
        var response = (await _context.AddAsync(entity, cancellationToken).AsTask()).Entity;
        EntityState = EntityState.Changed;
        return response;
    }

    public override async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await _context.AddRangeAsync(entities, cancellationToken);
        EntityState = EntityState.Changed;
    }

    public override Task CommitAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.CommitAsync(cancellationToken);

    public override async ValueTask DisposeAsync() => await _context.DisposeAsync();

    public override void Dispose() => _context.Dispose();

    public override Task<TEntity?> FindAsync(
        IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> fields = new(keyValues);
        return _context.Set<TEntity>().IgnoreQueryFilters().GetQueryable(fields).FirstOrDefaultAsync(cancellationToken);
    }

    public override Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().LongCountAsync(cancellationToken);

    public override Task<long> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().LongCountAsync(predicate, cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sorting">asc or desc, default asc</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TEntity>> GetPaginatedListAsync(
        int skip,
        int take,
        Dictionary<string, bool>? sorting,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return _context.Set<TEntity>().OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="predicate">condition</param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sorting">asc or desc, default asc</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TEntity>> GetPaginatedListAsync(
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return _context.Set<TEntity>().Where(predicate).OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public override Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Remove(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken);
        EntityState = EntityState.Changed;
        _context.Set<TEntity>().RemoveRange(entities);
    }

    public override Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        EntityState = EntityState.Changed;
        return Task.CompletedTask;
    }

    public override Task RollbackAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.RollbackAsync(cancellationToken);

    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().UpdateRange(entities);
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
    IRepository<TEntity, TKey>, IUnitOfWork
    where TEntity : class, IEntity<TKey>
    where TDbContext : DbContext
    where TKey : IComparable
{
    public Repository(TDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<TEntity?> FindAsync(TKey id)
        => _context.Set<TEntity>().FirstOrDefaultAsync(entity => entity.Id.Equals(id));
}
