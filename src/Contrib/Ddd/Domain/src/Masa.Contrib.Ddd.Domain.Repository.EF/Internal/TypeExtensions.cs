// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Ddd.Domain.Repository.EF.Internal;

internal static class TypeExtensions
{
    public static bool IsConcrete(this Type type) => !type.GetTypeInfo().IsAbstract && !type.GetTypeInfo().IsInterface;

    public static bool IsGenericInterfaceAssignableFrom(this Type eventHandlerType, Type type) =>
        type.IsConcrete() &&
        type.GetInterfaces().Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == eventHandlerType);
}
