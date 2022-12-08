// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.Linq;

public static class QueryableExtensions
{
    public static async Task<PaginatedListBase<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, bool>> predicate,
        PaginatedOptions options,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var result = await GetPaginatedListAsync(
            queryable,
            predicate,
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken);

        var total = await queryable.LongCountAsync(predicate, cancellationToken);

        return new PaginatedListBase<TEntity>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    public static async Task<PaginatedListBase<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        PaginatedOptions options,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        var result = await GetPaginatedListAsync(
            queryable,
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken);

        var total = await queryable.LongCountAsync(cancellationToken);

        return new PaginatedListBase<TEntity>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    private static async Task<List<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, bool>> predicate,
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        sorting ??= new Dictionary<string, bool>();

        return await queryable.Where(predicate).OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }

    private static async Task<List<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        int skip,
        int take,
        Dictionary<string, bool>? sorting = null,
        CancellationToken cancellationToken = default) where TEntity : class
    {
        sorting ??= new Dictionary<string, bool>();

        return await queryable.OrderBy(sorting).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }
}
