// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public static class SubscribeHelper
{
    /// <summary>
    /// Formats the subscribe channel.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="type">The type.</param>
    /// <param name="prefix">The prefix.</param>
    /// <returns>A string.</returns>
    public static string FormatSubscribeChannel<T>(string key, SubscribeKeyType type, string prefix = "")
    {
        var valueTypeFullName = typeof(T).FullName!;
        switch (type)
        {
            case SubscribeKeyType.ValueTypeFullName:
                return valueTypeFullName;
            case SubscribeKeyType.ValueTypeFullNameAndKey:
                return $"[{valueTypeFullName}]{key}";
            case SubscribeKeyType.SpecificPrefix:
                return $"{prefix}{key}";
            default:
                throw new NotImplementedException();
        }
    }
}
