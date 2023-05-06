// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

/// <summary>
/// The object that dapr sends integration events
/// when isolation is used, use this object to send events
/// </summary>
[ExcludeFromCodeCoverage]
internal class IntegrationEventMessage : IntegrationEventExpand
{
    /// <summary>
    /// Raw integration event information
    /// </summary>
    public object? Data { get; set; }

    public IntegrationEventMessage(object? @event, IntegrationEventExpand? eventMessageExpand)
    {
        Data = @event;

        if (eventMessageExpand == null)
            return;

        Isolation = eventMessageExpand.Isolation;
    }
}
