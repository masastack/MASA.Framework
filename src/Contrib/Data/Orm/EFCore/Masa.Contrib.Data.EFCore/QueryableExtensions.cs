// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.EFCore;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, bool>> predicate,
        PaginatedOptions options,
        CancellationToken cancellationToken = default) where TEntity : class, IEntity
    {

        var result = await GetPaginatedListAsync(
            queryable,
            predicate,
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken);

        var total = await queryable.LongCountAsync(predicate, cancellationToken);

        return new PaginatedList<TEntity>()
        {
            Total = total,
            Result = result,
            TotalPages = (int)Math.Ceiling(total / (decimal)options.PageSize)
        };
    }

    public static async Task<PaginatedList<TEntity>> GetPaginatedListAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        PaginatedOptions options,
        CancellationToken cancellationToken = default) where TEntity : class, IEntity
    {

        var result = await GetPaginatedListAsync(
            queryable,
            (options.Page - 1) * options.PageSize,
            options.PageSize <= 0 ? int.MaxValue : options.PageSize,
            options.Sorting,
            cancellationToken);

        var total = await queryable.LongCountAsync(cancellationToken);

        return new PaginatedList<TEntity>()
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

    private static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, Dictionary<string, bool> fields) where TEntity : class
    {
        var index = 0;
        foreach (var field in fields)
        {
            if (index == 0)
                query = query.OrderBy(field.Key, field.Value);
            else
                query = query.ThenBy(field.Key, field.Value);
            index++;
        }

        return query;
    }

    private static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, string field, bool desc) where TEntity : class
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity));
        Expression key = Expression.Property(parameterExpression, field);
        var propertyInfo = GetPropertyInfo(typeof(TEntity), field);
        var orderExpression = GetOrderExpression(typeof(TEntity), propertyInfo);
        if (desc)
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(TEntity), propertyInfo.PropertyType);
            return (genericMethod.Invoke(null, new object[] { query, orderExpression }) as IQueryable<TEntity>)!;
        }
        else
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(TEntity), propertyInfo.PropertyType);
            return (IQueryable<TEntity>)genericMethod.Invoke(null, new object[] { query, orderExpression })!;
        }
    }

    private static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string field, bool desc) where T : class
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
        Expression key = Expression.Property(parameterExpression, field);
        var propertyInfo = GetPropertyInfo(typeof(T), field);
        var orderExpression = GetOrderExpression(typeof(T), propertyInfo);
        if (desc)
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
            return (genericMethod.Invoke(null, new object[] { query, orderExpression }) as IQueryable<T>)!;
        }
        else
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "ThenBy" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, orderExpression })!;
        }
    }

    private static PropertyInfo GetPropertyInfo(Type entityType, string field)
        => entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(field, StringComparison.OrdinalIgnoreCase))!;

    private static LambdaExpression GetOrderExpression(Type entityType, PropertyInfo propertyInfo)
    {
        var parametersExpression = Expression.Parameter(entityType);
        var fieldExpression = Expression.PropertyOrField(parametersExpression, propertyInfo.Name);
        return Expression.Lambda(fieldExpression, parametersExpression);
    }
}
