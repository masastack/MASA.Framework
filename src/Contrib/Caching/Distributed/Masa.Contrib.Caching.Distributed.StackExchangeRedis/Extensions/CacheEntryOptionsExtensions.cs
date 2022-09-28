// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.BuildingBlocks.Caching;

public static class CacheEntryOptionsExtensions
{
    public static DateTimeOffset? GetAbsoluteExpiration(this CacheEntryOptions options, DateTimeOffset creationTime)
    {
        if (options.AbsoluteExpiration.HasValue && options.AbsoluteExpiration <= creationTime)
            throw new ArgumentOutOfRangeException(
                nameof(options),
                options.AbsoluteExpiration.Value,
                "The absolute expiration value must be in the future.");

        if (options.AbsoluteExpirationRelativeToNow.HasValue)
            return creationTime.Add(options.AbsoluteExpirationRelativeToNow.Value);

        return options.AbsoluteExpiration;
    }
}
