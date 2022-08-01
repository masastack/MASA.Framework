// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler;

public interface ISchedulerClient
{
    ISchedulerJobService SchedulerJobService { get; }

    ISchedulerTaskService SchedulerTaskService { get; }
}
