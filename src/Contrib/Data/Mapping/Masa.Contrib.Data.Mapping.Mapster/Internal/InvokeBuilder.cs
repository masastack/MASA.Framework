// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Internal;

internal static class InvokeBuilder
{
    private static readonly MethodInfo _newConfigMethodInfo;
    private static readonly Type _typeAdapterConfigType;

    static InvokeBuilder()
    {
        var typeAdapterSetterExpandType = typeof(TypeAdapterSetterExpand);
        _newConfigMethodInfo = typeAdapterSetterExpandType.GetMethod(nameof(TypeAdapterSetterExpand.NewConfigByConstructor))!;
        _typeAdapterConfigType = typeof(TypeAdapterConfig);
    }

    internal delegate TypeAdapterSetter MethodExecutor(TypeAdapterConfig target, object parameter);

    public static MethodExecutor Build(
        Type sourceType,
        Type destinationType)
    {
        var methodInfo = _newConfigMethodInfo.MakeGenericMethod(sourceType, destinationType);

        ParameterExpression[] parameters =
        {
            Expression.Parameter(_typeAdapterConfigType, "adapterConfigParameter"),
            Expression.Parameter(typeof(object), "constructorInfoParameter")
        };
        var newConfigMethodCall = Expression.Call(
            null,
            methodInfo,
            parameters
        );

        var lambda = Expression.Lambda<MethodExecutor>(newConfigMethodCall, parameters);
        return lambda.Compile();
    }
}
