namespace MASA.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogContext : MasaDbContext
{
    public IntegrationEventLogContext(
        MasaDbContextOptions? options = null,
        MasaDbContextOptions<IntegrationEventLogContext>? eventLogContext = null)
        : base(eventLogContext ?? options ??
            throw new InvalidOperationException("Options extension of type 'CoreOptionsExtension' not found"))
    {
    }

    public DbSet<IntegrationEventLog> EventLogs { get; set; }

    protected override void OnModelCreatingExecuting(ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLog>(ConfigureEventLogEntry);
    }

    private void ConfigureEventLogEntry(EntityTypeBuilder<IntegrationEventLog> builder)
    {
        builder.ToTable("IntegrationEventLog");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .IsRequired();

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.RowVersion)
            .IsRowVersion();

        builder.Property(e => e.EventTypeName)
            .IsRequired();

        builder.HasIndex(e => new { e.State, e.TimesSent });
    }
}
