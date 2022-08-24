// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public class SubscribeHelper
{
    /// <summary>
    /// Formats the subscribe channel.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="type">The type.</param>
    /// <param name="prefix">The prefix.</param>
    /// <returns>A string.</returns>
    public static string FormatSubscribeChannel<T>(string key, SubscribeKeyTypes type, string prefix = "")
    {
        var valueTypeFullName = typeof(T).FullName!;
        switch (type)
        {
            case SubscribeKeyTypes.ValueTypeFullName:
                return valueTypeFullName;
            case SubscribeKeyTypes.ValueTypeFullNameAndKey:
                return $"[{valueTypeFullName}]{key}";
            case SubscribeKeyTypes.SpecificPrefix:
                return $"{prefix}{key}";
            default:
                throw new NotImplementedException();
        }
    }
}
