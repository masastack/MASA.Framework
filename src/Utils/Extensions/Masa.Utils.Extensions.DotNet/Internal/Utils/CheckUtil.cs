// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
internal static class CheckUtil
{
    internal static T CheckArgumentNull<T>(T value, string parameterName) where T : class
    {
        if (null == value)
            throw new ArgumentNullException(parameterName);

        return value;
    }
}
