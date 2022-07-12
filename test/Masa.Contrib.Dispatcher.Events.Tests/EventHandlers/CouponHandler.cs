// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class CouponHandler
{
    [EventHandler]
    public async Task SendCouponAsync(SendCouponEvent @event)
    {
        throw new UserFriendlyException("custom exception");
    }
}
