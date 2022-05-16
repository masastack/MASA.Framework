// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Internal;

internal class ObjectStorageExtensions
{
    internal static string GetEndpoint(string regionId, EndpointMode mode)
        => regionId + (mode == EndpointMode.Public ? Const.PUBLIC_ENDPOINT_DOMAIN_SUFFIX : Const.INTERNAL_ENDPOINT_SUFFIX);

    internal static string CheckNullOrEmptyAndReturnValue(string? parameter, string parameterName)
    {
        if (string.IsNullOrEmpty(parameter))
            throw new ArgumentException($"{parameterName} cannot be null and empty string");

        return parameter;
    }
}
