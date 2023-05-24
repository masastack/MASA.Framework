// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.Events.Tests.Events;

public record ComputeEvent : Event
{
    /// <summary>
    /// the unit price
    /// </summary>
    public decimal Price { get; set; }

    public int Count { get; set; }

    /// <summary>
    /// original price
    /// </summary>
    public decimal Amount => Price * Count;

    /// <summary>
    /// preferential price
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// actual amount paid
    /// </summary>
    public decimal PayAmount { get; set; }
}
