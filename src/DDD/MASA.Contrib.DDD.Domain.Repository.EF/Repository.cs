namespace MASA.Contrib.DDD.Domain.Repository.EF;

public class Repository<TDbContext, TEntity> : BaseRepository<TEntity>
    where TEntity : class, IAggregateRoot
    where TDbContext : DbContext
{
    protected readonly TDbContext _context;

    public Repository(TDbContext context, IUnitOfWork unitOfWork)
    {
        _context = context;
        UnitOfWork = unitOfWork;
    }

    public override DbTransaction Transaction
    {
        get
        {
            if (TransactionHasBegun)
            {
                return _context.Database.CurrentTransaction!.GetDbTransaction();
            }
            return _context.Database.BeginTransaction().GetDbTransaction();
        }
        set
        {
            if (_context.Database.CurrentTransaction == null)
            {
                _context.Database.UseTransaction(value);
            }
            else
            {
                throw new NotSupportedException("The transaction is opened");
            }
        }
    }

    public override IUnitOfWork UnitOfWork { get; set; }

    public override async ValueTask<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => (await _context.AddAsync(entity, cancellationToken).AsTask()).Entity;

    public override Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => _context.AddRangeAsync(entities, cancellationToken);

    public override Task CommitAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.CommitAsync(cancellationToken);

    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public override ValueTask<TEntity?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
        => _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);

    public override Task<TEntity?> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _context.Set<TEntity>().Where(predicate).FirstOrDefaultAsync(cancellationToken);

    public override Task<long> GetCountAsync(CancellationToken cancellationToken)
        => _context.Set<TEntity>().LongCountAsync(cancellationToken);

    public override Task<long> GetCountAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
        => _context.Set<TEntity>().LongCountAsync(predicate, cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(CancellationToken cancellationToken)
        => await _context.Set<TEntity>().ToListAsync(cancellationToken);

    public override async Task<IEnumerable<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
        => await _context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    public override Task<List<TEntity>> GetPaginatedListAsync(int skip, int take, string? sorting, CancellationToken cancellationToken)
    {
        var iQueryable = _context.Set<TEntity>();
        return (string.Equals(sorting ?? "asc", "asc", StringComparison.CurrentCultureIgnoreCase) ? iQueryable.OrderBy(x => x.GetKeys()) : iQueryable.OrderByDescending(x => x.GetKeys())).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public override Task<List<TEntity>> GetPaginatedListAsync(Expression<Func<TEntity, bool>> predicate, int skip, int take, string? sorting, CancellationToken cancellationToken)
    {
        var iQueryable = _context.Set<TEntity>().Where(predicate);
        return (string.Equals(sorting ?? "asc", "asc", StringComparison.CurrentCultureIgnoreCase) ? iQueryable.OrderBy(x => x.GetKeys()) : iQueryable.OrderByDescending(x => x.GetKeys())).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    public override Task<TEntity> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Remove(entity);
        return Task.FromResult(entity);
    }

    public override async Task RemoveAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entities = await GetListAsync(predicate, cancellationToken)!;
        _context.Set<TEntity>().RemoveRange(entities);
    }

    public override Task RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().RemoveRange(entities);
        return Task.CompletedTask;
    }

    public override Task RollbackAsync(CancellationToken cancellationToken = default)
        => UnitOfWork.RollbackAsync(cancellationToken);

    public override Task SaveChangesAsync(CancellationToken cancellationToken = default)
         => UnitOfWork.SaveChangesAsync(cancellationToken);

    public override Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().Update(entity);
        return Task.FromResult(entity);
    }

    public override Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        _context.Set<TEntity>().UpdateRange(entities);
        return Task.CompletedTask;
    }
}
