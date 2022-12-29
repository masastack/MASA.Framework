// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public static class EntityTypeGlobalConfiguration
{
    public static Func<Type, string>? SchemaFunc { get; set; } = null;

    public static Func<Type, string>? TableFunc { get; set; } = null;

    /// <summary>
    /// Set whether the primary key is auto-incremented
    /// </summary>
    public static Func<Type, bool>? SelfIncrementFunc { get; set; } = null;

    /// <summary>
    /// Database table, list naming rules, used when no database table name, list is specified
    /// </summary>
    public static DatabaseNamingRules? DatabaseNamingRules { get; set; } = null;
}
