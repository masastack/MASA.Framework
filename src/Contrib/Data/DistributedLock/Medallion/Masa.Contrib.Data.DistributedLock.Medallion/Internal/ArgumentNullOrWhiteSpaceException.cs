// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.DistributedLock.Medallion.Internal;

internal static class ArgumentNullOrWhiteSpaceException
{
    public static void ThrowIfNullOrWhiteSpace(string? argument, string? paramName = null)
    {
        paramName ??= nameof(argument);

        if (string.IsNullOrWhiteSpace(argument))
            throw new ArgumentException($"{paramName} can not be null, empty or white space!", paramName);
    }
}
