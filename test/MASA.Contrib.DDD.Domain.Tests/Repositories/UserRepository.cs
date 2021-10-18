namespace MASA.Contrib.DDD.Domain.Tests.Repositories;

public class UserRepository : IRepository<User>
{
    public IUnitOfWork UnitOfWork => throw new NotImplementedException();

    public ValueTask<User> AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task AddRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<User?> FindAsync(params object?[]? keyValues)
    {
        throw new NotImplementedException();
    }

    public ValueTask<User?> FindAsync(object?[]? keyValues, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User?> FindAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetCountAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<long> GetCountAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetListAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetPaginatedListAsync(int skip, int take, string? sorting, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetPaginatedListAsync(Expression<Func<User, bool>> predicate, int skip, int take, string? sorting, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<User>> GetPaginatedListAsync(PaginatedOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<User>> GetPaginatedListAsync(Expression<Func<User, bool>> predicate, PaginatedOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<User> RemoveAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<User> UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateRangeAsync(IEnumerable<User> entities, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
