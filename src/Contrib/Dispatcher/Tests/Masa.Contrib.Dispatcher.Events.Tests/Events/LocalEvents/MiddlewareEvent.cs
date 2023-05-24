// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

/// <summary>
/// Events for testing middleware execution order
/// </summary>
public record MiddlewareEvent : Event
{
    /// <summary>
    /// Class name used to store execution middleware and Handler
    /// </summary>
    public List<string> Results { get; set; } = new();
}
