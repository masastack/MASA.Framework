// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.EventLogs.EntityFrameworkCore;

public class IntegrationEventLogContext
{
    public readonly DbContext DbContext;

    public IntegrationEventLogContext(DbContext dbContext) => DbContext = dbContext;

    public DbSet<IntegrationEventLog> EventLogs => DbContext.Set<IntegrationEventLog>();
}
