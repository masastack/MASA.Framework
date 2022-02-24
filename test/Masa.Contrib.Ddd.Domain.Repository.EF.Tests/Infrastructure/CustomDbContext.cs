namespace Masa.Contrib.Ddd.Domain.Repository.EF.Tests.Infrastructure;

public class CustomDbContext : DbContext
{
    public DbSet<Orders> Orders { get; set; }

    public DbSet<Students> Students { get; set; }

    public DbSet<Courses> Courses { get; set; }

    public DbSet<Hobbies> Hobbies { get; set; }

    public CustomDbContext(DbContextOptions<CustomDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Students>(
            entityTypeBuilder =>
            {
                entityTypeBuilder.HasKey("SerialNumber");
            });
    }
}
