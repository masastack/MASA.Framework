// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events;

/// <summary>
/// Used to filter EventBus middleware that does not support recursion
/// </summary>
internal interface IInitializeServiceProvider
{
    /// <summary>
    /// Get the initialization state. If it has been initialized, the middleware that does not support recursion is no longer executed.
    /// </summary>
    bool IsInitialize { get; }

    /// <summary>
    /// service has been initialized
    /// </summary>
    void Initialize();

    /// <summary>
    /// reset initialization state
    /// </summary>
    void Reset();
}
