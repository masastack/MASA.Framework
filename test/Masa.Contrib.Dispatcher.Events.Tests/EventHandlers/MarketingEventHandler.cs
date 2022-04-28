// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Events.Tests.EventHandlers;

public class MarketingEventHandler
{
    [EventHandler(10, true, false)]
    public void Discount(ComputeEvent computeEvent)
    {
        var discountRate = 0.7m;
        computeEvent.DiscountAmount = computeEvent.Amount * (1 - discountRate);
        computeEvent.PayAmount = computeEvent.Amount * discountRate;
    }

    [EventHandler(20)]
    public void FullReduction(ComputeEvent computeEvent)
    {
        var discounts = 0;
        if (computeEvent.PayAmount > 200)
        {
            discounts = 50;
            computeEvent.DiscountAmount += discounts;
        }
        computeEvent.PayAmount -= discounts;
    }
}
