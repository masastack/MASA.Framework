namespace MASA.Contrib.ReadWriteSpliting.CQRS.Tests;

public class ProductionQueryHandler : QueryHandler<ProductionItemQuery, string>
{
    public override Task HandleAsync(ProductionItemQuery @event)
    {
        if (string.IsNullOrEmpty(@event.ProductionId))
            throw new ArgumentNullException(nameof(@event));

        if (@event.Id == default(Guid) || @event.CreationTime > DateTime.UtcNow)
            throw new ArgumentNullException(nameof(@event));

        if (@event.ProductionId == "1")
            @event.Result = "Apple";

        return Task.CompletedTask;
    }
}
