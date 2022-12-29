// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace System;

public enum DatabaseNamingRules
{
    /// <summary>
    /// Example: User_Name
    /// </summary>
    SnakeCase = 1,

    /// <summary>
    /// Example: user_name
    /// </summary>
    LowerSnakeCase,

    /// <summary>
    /// Example: UserName
    /// </summary>
    CamelCase,

    /// <summary>
    /// Example: userName
    /// </summary>
    LowerCamelCase
}
