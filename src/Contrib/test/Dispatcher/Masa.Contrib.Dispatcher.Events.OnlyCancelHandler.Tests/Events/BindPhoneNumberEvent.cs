// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Dispatcher.Events;

namespace Masa.Contrib.Dispatcher.Events.OnlyCancelHandler.Tests.Events;

public record BindPhoneNumberEvent : Event
{
    public string AccountId { get; set; }

    public string PhoneNumber { get; set; }
}
