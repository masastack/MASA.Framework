// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Contracts.EntityFrameworkCore.Internal;

internal class InstanceBuilder
{
    internal delegate object InvokeDelegate(params object?[] parameters);

    public static InvokeDelegate CreateInstanceDelegate(ConstructorInfo constructorInfo)
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
        var lambdaExp = Expression.Lambda<InvokeDelegate>(newExp, parameterParameterExpression);
        return lambdaExp.Compile();
    }
}
