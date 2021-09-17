namespace MASA.Contrib.Dispatcher.InMemory.Tests.EventHandlers;

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