namespace Masa.Contrib.Data.Contracts.EF.SoftDelete;

public class QueryFilterProvider : IQueryFilterProvider
{
    public LambdaExpression OnExecuting(IMutableEntityType entityType)
    {
        var parameter = Expression.Parameter(entityType.ClrType);
        if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
        {
            var propertyMethodInfo = typeof(Microsoft.EntityFrameworkCore.EF).GetMethod("Property")!.MakeGenericMethod(typeof(bool));
            var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant(nameof(ISoftDelete.IsDeleted)));
            var compareExpression = Expression.MakeBinary(ExpressionType.Equal, isDeletedProperty, Expression.Constant(false));
            var lambda = Expression.Lambda(compareExpression, parameter);
            return lambda;
        }

        return default!;
    }
}
