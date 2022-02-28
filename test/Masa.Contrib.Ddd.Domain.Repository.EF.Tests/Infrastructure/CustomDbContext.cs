namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Infrastructure;

public class CustomDbContext : MasaDbContext
{
    public DbSet<Orders> Orders { get; set; }

    public CustomDbContext(MasaDbContextOptions<CustomDbContext> options) : base(options) { }
}
