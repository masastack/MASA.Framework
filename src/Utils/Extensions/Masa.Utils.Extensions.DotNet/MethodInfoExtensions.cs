// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class MethodInfoExtensions
{
    public static bool IsAsyncMethod(this MethodInfo methodInfo)
    {
        var returnType = methodInfo.ReturnType;
        return returnType == typeof(Task) || (returnType.IsGenericType && returnType.GetInterfaces().Any(type => type == typeof(IAsyncResult)));
    }
}
