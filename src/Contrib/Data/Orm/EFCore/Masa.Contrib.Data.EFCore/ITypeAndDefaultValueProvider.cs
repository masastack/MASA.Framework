// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.EntityFrameworkCore;

public interface ITypeAndDefaultValueProvider
{
    /// <summary>
    /// Setting type and default value
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    bool TryAdd(Type type);

    bool TryGet(Type type, [NotNullWhen(true)] out string? defaultValue);
}
