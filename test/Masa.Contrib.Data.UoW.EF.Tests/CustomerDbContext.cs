namespace Masa.Contrib.Data.UoW.EF.Tests;

public class CustomerDbContext : MasaDbContext
{
    private static readonly Mock<MasaDbContextOptions> masaDbContextOptions = new();
    public CustomerDbContext() : this(masaDbContextOptions.Object)
    {
    }

    public CustomerDbContext(MasaDbContextOptions options) : base(options) { }

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
    public Guid Id { get; private set; }

    public string Name { get; set; } = default!;

    public Users()
    {
        this.Id = Guid.NewGuid();
    }
}
