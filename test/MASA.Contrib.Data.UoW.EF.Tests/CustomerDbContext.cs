using MASA.Utils.Data.EntityFrameworkCore;

namespace MASA.Contrib.Data.UoW.EF.Tests;

public class CustomerDbContext : MasaDbContext
{
    public CustomerDbContext(MasaDbContextOptions<CustomerDbContext> options) : base(options) { }

    public DbSet<Users> User { get; set; }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.Entity<Users>(ConfigureUserEntry);
    }

    void ConfigureUserEntry(EntityTypeBuilder<Users> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Name)
            .HasMaxLength(6)
            .IsRequired();
    }
}

public class Users
{
    public Guid Id { get; set; }

    public string Name { get; set; }
}
