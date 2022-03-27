namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogModelCreatingProvider : IModelCreatingProvider
{
    public void Configure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IntegrationEventLog>(ConfigureEventLogEntry);
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

        builder.Property(e => e.ModificationTime)
            .IsRequired();

        builder.Property(e => e.State)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder.Property(e => e.RowVersion)
            .HasMaxLength(36)
            .IsRequired();

        builder.Property(e => e.EventTypeName)
            .IsRequired();

        builder.HasIndex(e => new { e.State, e.ModificationTime }, "index_state_modificationtime");
        builder.HasIndex(e => new { e.State, e.TimesSent, e.ModificationTime }, "index_state_timessent_modificationtime");
        builder.HasIndex(e => new { e.EventId, e.RowVersion }, "index_eventid_version");
    }
}
