// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

internal static class ObjectStorageExtensions
{
    internal static string CheckNullOrEmptyAndReturnValue(string? parameter, string parameterName)
    {
        MasaArgumentException.ThrowIfNullOrWhiteSpace(parameter, parameterName);

        return parameter!;
    }
}
