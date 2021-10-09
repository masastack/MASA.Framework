namespace MASA.Contribs.DDD.Domain.Repository.EF.Tests.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Orders> Orders { get; set; }
}
