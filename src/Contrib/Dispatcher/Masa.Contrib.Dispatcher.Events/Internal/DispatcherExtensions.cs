// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Internal;

internal static class DispatcherExtensions
{
    public static bool IsGenericInterfaceAssignableFrom(this Type genericInterfaceType, Type genericImplementationType)
    {
        if (!genericImplementationType.IsGenericTypeDefinition ||
            genericImplementationType.IsAbstract ||
            genericImplementationType.IsInterface)
            return false;

        return genericImplementationType.GetInterfaces().Any(type => type.GetGenericTypeDefinition() == genericInterfaceType);
    }

    /// <summary>
    /// Keep the original stack information and throw an exception
    /// </summary>
    /// <param name="exception"></param>
    public static void ThrowException(this Exception exception)
    {
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}
