// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Caching;

public enum CacheKeyType
{
    /// <summary>
    /// Keep it the same, use the key directly
    /// </summary>
    None = 1,

    /// <summary>
    /// Type's name(Type's full name with generic type name) and key combination
    /// </summary>
    TypeName,

    /// <summary>
    /// Type Alias and key combination, Format: ${TypeAliasName}{:}{key}
    /// </summary>
    TypeAlias
}
