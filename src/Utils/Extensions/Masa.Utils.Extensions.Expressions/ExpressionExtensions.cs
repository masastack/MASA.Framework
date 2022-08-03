// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System.Linq.Expressions;

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.And);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, bool isCompose, Expression<Func<T, bool>>? second)
    {
        if (isCompose && second != null)
            return first.Compose(second, Expression.And);

        return first;
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        return first.Compose(second, Expression.Or);
    }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, bool isCompose, Expression<Func<T, bool>>? second)
    {
        if (isCompose && second != null)
            return first.Compose(second, Expression.Or);

        return first;
    }

    public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        var parameterMap = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

        var secondBody = ParameterRebinder.ReplaceParameters(parameterMap, second.Body);

        return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
    }

    #region GetMemberName

    public static List<string> GetMemberName<T>(this Expression<T> expression)
        where T : BinaryExpression
    {
        var result = new List<string>();
        var expressionBody = expression.Body;
        if (expressionBody is BinaryExpression)
        {
            result.AddRange(ParseBinaryExpression(expressionBody));
        }

        return result;
    }

    private static List<string> ParseBinaryExpression(Expression expression)
    {
        var result = new List<string>();
        var binaryExpression = expression as BinaryExpression;

        if (binaryExpression!.Left != null)
            result.AddRange(ParseUnitExpression(binaryExpression.Left));

        if (binaryExpression.Right != null)
            result.AddRange(ParseUnitExpression(binaryExpression.Right));

        return result;
    }

    private static List<string> ParseUnitExpression(Expression unitExpression)
    {
        var result = new List<string>();

        if (unitExpression is BinaryExpression)
        {
            result.AddRange(ParseBinaryExpression((unitExpression as BinaryExpression)!));
        }
        else if (unitExpression is MemberExpression)
        {
            var memberExpression = unitExpression as MemberExpression;
            var pi = memberExpression!.Member as PropertyInfo;
            result.Add(pi!.Name);
        }

        return result;
    }

    #endregion

}

public class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> map;

    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    protected override Expression VisitParameter(ParameterExpression p)
    {
        ParameterExpression replacement;
        if (map.TryGetValue(p, out replacement!))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }
}
