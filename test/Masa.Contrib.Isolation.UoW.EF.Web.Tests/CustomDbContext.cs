namespace Masa.Contrib.Isolation.UoW.EF.Web.Tests;

public class CustomDbContext : IsolationDbContext
{
    public CustomDbContext(MasaDbContextOptions options) : base(options) { }

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

        builder.Property(e => e.Account)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Account)
            .HasMaxLength(50)
            .IsRequired();
    }
}

public class Users : IIsolation<int>
{
    public Guid Id { get; private set; }

    public string Account { get; set; } = default!;

    public string Password { get; set; } = default!;

    public Users()
    {
        this.Id = Guid.NewGuid();
    }

    public int TenantId { get; set; }

    public string Environment { get; set; }
}
