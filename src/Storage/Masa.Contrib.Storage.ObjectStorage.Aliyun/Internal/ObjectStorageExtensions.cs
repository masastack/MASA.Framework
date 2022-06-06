// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal;

internal class ObjectStorageExtensions
{
    internal static string CheckNullOrEmptyAndReturnValue(string? parameter, string parameterName)
    {
        if (string.IsNullOrEmpty(parameter))
            throw new ArgumentException($"{parameterName} cannot be null and empty string");

        return parameter;
    }
}
