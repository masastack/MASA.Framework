// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Exceptions.Internal;

internal static class ExceptionHandlerExtensions
{
    public static IMasaExceptionHandler? GetMasaExceptionHandler(IServiceProvider serviceProvider, Type? masaExceptionHandlerType)
    {
        var exceptionHandler = serviceProvider.GetService<IMasaExceptionHandler>();
        if (exceptionHandler != null)
            return exceptionHandler;

        if (masaExceptionHandlerType == null)
            return null;

        var constructor = masaExceptionHandlerType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .MaxBy(c => c.GetParameters().Length);
        List<object?> parameters = new();
        if (constructor != null)
        {
            foreach (var parameterInfo in constructor.GetParameters())
                parameters.Add(serviceProvider.GetService(parameterInfo.ParameterType));
        }
        var instance = Activator.CreateInstance(masaExceptionHandlerType, parameters.ToArray());
        if (instance != null)
            return instance as IMasaExceptionHandler;

        return null;
    }
}
