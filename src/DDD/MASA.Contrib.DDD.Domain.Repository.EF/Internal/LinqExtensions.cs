namespace MASA.Contrib.DDD.Domain.Repository.EF.Internal;

internal static class LinqExtensions
{
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string[] fields, bool desc) where T : class
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (i == 0)
                query = query.OrderBy(fields[i], desc);
            else
                query = query.ThenBy(fields[i], desc);
        }

        return query;
    }

    public static IQueryable<T> OrderBy<T>(this IQueryable<T> query, string field, bool desc) where T : class
    {
        ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
        Expression key = Expression.Property(parameterExpression, field);
        var propertyInfo = GetPropertyInfo(typeof(T), field);
        var orderExpression = GetOrderExpression(typeof(T), propertyInfo);
        if (desc)
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
            return (genericMethod.Invoke(null, new object[] { query, orderExpression }) as IQueryable<T>)!;
        }
        else
        {
            var method = typeof(Queryable).GetMethods().FirstOrDefault(m => m.Name == "OrderBy" && m.GetParameters().Length == 2);
            var genericMethod = method!.MakeGenericMethod(typeof(T), propertyInfo.PropertyType);
            return (IQueryable<T>)genericMethod.Invoke(null, new object[] { query, orderExpression })!;
        }
    }

    public static IQueryable<T> ThenBy<T>(this IQueryable<T> query, string field, bool desc) where T : class
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
