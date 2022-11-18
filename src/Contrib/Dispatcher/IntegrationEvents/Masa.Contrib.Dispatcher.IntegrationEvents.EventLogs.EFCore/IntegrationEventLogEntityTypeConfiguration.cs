// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EFCore;

public class IntegrationEventLogEntityTypeConfiguration: IEntityTypeConfiguration<IntegrationEventLog>
{
    public void Configure(EntityTypeBuilder<IntegrationEventLog> builder)
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

        builder.TryConfigureConcurrencyStamp(nameof(IHasConcurrencyStamp.RowVersion));

        builder.Property(e => e.EventTypeName)
            .IsRequired();

        builder.HasIndex(e => new { e.State, e.ModificationTime }, "IX_State_MTime");
        builder.HasIndex(e => new { e.State, e.TimesSent, e.ModificationTime }, "IX_State_TimesSent_MTime");
        builder.HasIndex(e => new { e.EventId, e.RowVersion }, "IX_EventId_Version");
    }
}
