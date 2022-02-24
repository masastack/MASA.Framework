namespace Masa.Contrib.Ddd.Domain.Repository.EF.Internal;

internal static class LinqExtensions
{
    public static IQueryable<TEntity> GetQueryable<TEntity>(this IQueryable<TEntity> query, Dictionary<string, object> fields) where TEntity : class
    {
        foreach (var field in fields)
        {
            query = query.GetQueryable(field.Key, field.Value);
        }
        return query;
    }

    private static IQueryable<TEntity> GetQueryable<TEntity>(this IQueryable<TEntity> query, string field, object val) where TEntity : class
    {
        Type type = typeof(TEntity);
        var parameter = Expression.Parameter(type, "entity");

        PropertyInfo property = type.GetProperty(field)!;
        Expression expProperty = Expression.Property(parameter, property.Name);

        Expression<Func<object>> valueLamda = () => val;
        Expression expValue = Expression.Convert(valueLamda.Body, property.PropertyType);
        Expression expression = Expression.Equal(expProperty, expValue);
        Expression<Func<TEntity, bool>> filter = (Expression<Func<TEntity, bool>>)Expression.Lambda(expression, parameter);
        return query.Where(filter);
    }

    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> query, Dictionary<string, bool> fields) where TEntity : class
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
