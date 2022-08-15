// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler;

public class SchedulerClient : ISchedulerClient
{
    public ISchedulerJobService SchedulerJobService { get; }

    public ISchedulerTaskService SchedulerTaskService { get; }

    public SchedulerClient(ICaller caller, ILoggerFactory? loggerFactory = null)
    {
        SchedulerJobService = new SchedulerJobService(caller, loggerFactory);
        SchedulerTaskService = new SchedulerTaskService(caller, loggerFactory);
    }
}
