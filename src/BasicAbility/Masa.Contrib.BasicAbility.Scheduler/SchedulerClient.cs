// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Scheduler;

public class SchedulerClient : ISchedulerClient
{
    public ISchedulerJobService SchedulerJobService { get; }

    public ISchedulerTaskService SchedulerTaskService { get; }

    public SchedulerClient(ICallerProvider callerProvider, ILoggerFactory loggerFactory)
    {
        SchedulerJobService = new SchedulerJobService(callerProvider, loggerFactory);
        SchedulerTaskService = new SchedulerTaskService(callerProvider, loggerFactory);
    }
}
