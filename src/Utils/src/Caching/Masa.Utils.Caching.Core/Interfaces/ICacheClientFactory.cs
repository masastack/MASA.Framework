// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.Interfaces;

/// <summary>
/// A factory abstraction for a component that can create instances of <typeparamref name="TICacheClinet" /> type with custom
/// configuration for a given logical name.
/// </summary>
public interface ICacheClientFactory<TICacheClinet>
{
    /// <summary>
    /// Creates and configures an instance of <typeparamref name="TICacheClinet" /> type using the configuration that corresponds
    /// to the logical name specified by <paramref name="name"/>.
    /// </summary>
    /// <param name="name">The logical name of the client to create.</param>
    /// <returns>A new instance of <typeparamref name="TICacheClinet" /> type.</returns>
    TICacheClinet CreateClient(string name);
}
