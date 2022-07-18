// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EF;

public class IntegrationEventLogModelCreatingProvider : IModelCreatingProvider
{
    public void Configure(ModelBuilder modelBuilder)
    {
        var integrationEventLogEntityTypeConfiguration = new IntegrationEventLogEntityTypeConfiguration();
        modelBuilder.Entity<IntegrationEventLog>(integrationEventLogEntityTypeConfiguration.Configure);
    }
}
