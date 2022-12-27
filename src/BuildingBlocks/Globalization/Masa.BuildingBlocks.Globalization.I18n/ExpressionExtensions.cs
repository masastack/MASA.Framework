// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Globalization.I18n;

public static class ExpressionExtensions
{
    public static string GetI18nName<TResource>(Expression<Func<TResource, string>> propertyExpression)
        => GetI18nName(propertyExpression.Body);

    private static string GetI18nName(Expression expression)
    {
        string name = string.Empty;
        if (expression is MemberExpression memberExpression)
        {
            name = memberExpression.Member.Name;

            if (memberExpression.Expression is { NodeType: ExpressionType.Parameter })
                return name;

            return $"{GetI18nName(memberExpression.Expression!)}.{name}";
        }

        return name;
    }
}
