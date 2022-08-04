// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore.Tests.Internal;

internal class Repository
{
    private readonly CustomDbContext _testDbContext;

    public Repository(CustomDbContext testDbContext) => _testDbContext = testDbContext;

    public Task<List<Student>> GetPaginatedListAsync(int skip, int take, CancellationToken cancellationToken = default)
        => _testDbContext.Set<Student>().Skip(skip).Take(take).ToListAsync(cancellationToken);

    public virtual async Task<BasePaginatedList<Student>> GetPaginatedListAsync(PaginatedOptions options, CancellationToken cancellationToken = default)
    {
        var result = await GetPaginatedListAsync(
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            cancellationToken
        );

        var total = await GetCountAsync(cancellationToken);

        return new BasePaginatedList<Student>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    public async Task<long> GetCountAsync(CancellationToken cancellationToken = default)
        => await _testDbContext.Set<Student>().LongCountAsync(cancellationToken);
}
