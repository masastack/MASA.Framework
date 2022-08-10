// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Authentication.Identity.Internal;

internal static class InstanceBuilder
{
    public static Func<object[], object> CreateInstanceDelegate(ConstructorInfo constructorInfo)
    {
        ParameterInfo[] parameters = constructorInfo.GetParameters();
        var parameterParameterExpression = Expression.Parameter(typeof(object[]), "parameters");

        var parameterCast = new List<Expression>(parameters.Length);
        for (int i = 0; i < parameters.Length; i++)
        {
            var paramInfo = parameters[i];
            var valueObj = Expression.ArrayIndex(parameterParameterExpression, Expression.Constant(i));
            var valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);
            parameterCast.Add(valueCast);
        }
        NewExpression newExp = Expression.New(constructorInfo, parameterCast);
        Expression<Func<object[], object>> lambdaExp = Expression.Lambda<Func<object[], object>>(newExp, parameterParameterExpression);
        return lambdaExp.Compile();
    }

    public static Dictionary<PropertyInfo, MethodInfo> GetPropertyAndMethodInfoRelations(Type type)
    {
        var properties = type.GetProperties();
        var propertyDict = new Dictionary<PropertyInfo, MethodInfo>(properties.Length);
        foreach (var property in properties)
        {
            var setMethod = property.GetSetMethod(property.PropertyType.IsNotPublic);
            if (setMethod != null) propertyDict.Add(property, setMethod);
        }
        return propertyDict;
    }
}
