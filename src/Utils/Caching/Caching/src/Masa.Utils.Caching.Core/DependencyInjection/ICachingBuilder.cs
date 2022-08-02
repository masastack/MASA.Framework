// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caching.Core.DependencyInjection;

/// <summary>
/// A builder for configuring named <see cref="ICachingBuilder"/> instances.
/// </summary>
public interface ICachingBuilder
{
    /// <summary>
    /// Gets the application service collection.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the name of the client configured by this builder.
    /// </summary>
    string Name { get; }
}
