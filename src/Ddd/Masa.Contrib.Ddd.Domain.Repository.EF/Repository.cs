namespace Masa.Contrib.Ddd.Domain.Repository.EF;

public class Repository<TDbContext, TAggregateRoot> :
    BaseRepository<TAggregateRoot>
    where TAggregateRoot : class, IAggregateRoot
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

    public override async ValueTask<TAggregateRoot> AddAsync(
        TAggregateRoot entity,
        CancellationToken cancellationToken = default)
    {
        var response = (await _context.AddAsync(entity, cancellationToken).AsTask()).Entity;
        EntityState = EntityState.Changed;
        return response;
    }

    public override async Task AddRangeAsync(
        IEnumerable<TAggregateRoot> entities,
        CancellationToken cancellationToken = default)
    {
        await _context.AddRangeAsync(entities, cancellationToken);
        EntityState = EntityState.Changed;
    }

    public override Task CommitAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.CommitAsync(cancellationToken);

    public override async ValueTask DisposeAsync() => await _context.DisposeAsync();

    public override void Dispose() => _context.Dispose();

    public override Task<TAggregateRoot?> FindAsync(
        IEnumerable<KeyValuePair<string, object>> keyValues,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> fields = new(keyValues);
        return _context.Set<TAggregateRoot>().IgnoreQueryFilters().GetQueryable(fields).FirstOrDefaultAsync(cancellationToken);
    }

    public override Task<TAggregateRoot?> FindAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TAggregateRoot>().Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public override async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        => await _context.Set<TAggregateRoot>().LongCountAsync(cancellationToken);

    public override Task<long> GetCountAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TAggregateRoot>().LongCountAsync(predicate, cancellationToken);

    public override async Task<IEnumerable<TAggregateRoot>> GetListAsync(CancellationToken cancellationToken = default)
        => await _context.Set<TAggregateRoot>().ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TAggregateRoot>> GetListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _context.Set<TAggregateRoot>().Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    ///
    /// </summary>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sorting">asc or desc, default asc</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<List<TAggregateRoot>> GetPaginatedListAsync(
        int skip,
        int take,
        Dictionary<string, bool>? sorting,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return _context.Set<TAggregateRoot>().OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
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
    public override Task<List<TAggregateRoot>> GetPaginatedListAsync(
        Expression<Func<TAggregateRoot, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting,
        CancellationToken cancellationToken = default)
    {
        sorting ??= new Dictionary<string, bool>();

        return _context.Set<TAggregateRoot>().Where(predicate).OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public override Task<TAggregateRoot> RemoveAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregateRoot>().Remove(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override async Task RemoveAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken);
        EntityState = EntityState.Changed;
        _context.Set<TAggregateRoot>().RemoveRange(entities);
    }

    public override Task RemoveRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregateRoot>().RemoveRange(entities);
        EntityState = EntityState.Changed;
        return Task.CompletedTask;
    }

    public override Task RollbackAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.RollbackAsync(cancellationToken);

    public override async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await UnitOfWork.SaveChangesAsync(cancellationToken);
    }

    public override Task<TAggregateRoot> UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregateRoot>().Update(entity);
        EntityState = EntityState.Changed;
        return Task.FromResult(entity);
    }

    public override Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TAggregateRoot>().UpdateRange(entities);
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

public class Repository<TDbContext, TAggregateRoot, TKey> :
    Repository<TDbContext, TAggregateRoot>,
    IRepository<TAggregateRoot, TKey>, IUnitOfWork
    where TAggregateRoot : class, IAggregateRoot<TKey>
    where TDbContext : DbContext
    where TKey : IComparable
{
    public Repository(TDbContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
    {
    }

    public Task<TAggregateRoot?> FindAsync(TKey id)
        => _context.Set<TAggregateRoot>().FirstOrDefaultAsync(entity => entity.Id!.Equals(id));
}
