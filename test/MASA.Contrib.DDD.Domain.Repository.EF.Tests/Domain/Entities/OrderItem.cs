namespace MASA.Contrib.DDD.Domain.Repository.EF.Tests.Domain.Entities;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public decimal UnitPrice { get; set; }

    public int Units { get; set; }

    public string PictureUrl { get; set; }
}
