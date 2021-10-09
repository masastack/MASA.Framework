namespace MASA.Contribs.DDD.Domain.Repository.EF.Tests.Domain.Entities;

public class Orders : AuditAggregateRoot<Guid, Guid>
{
    public int OrderNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public string OrderStatus { get; set; }

    public string Description { get; set; }

    public string BuyerId { get; set; }

    public string BuyerName { get; set; }

    public List<OrderItem> OrderItems { get; set; }
}

