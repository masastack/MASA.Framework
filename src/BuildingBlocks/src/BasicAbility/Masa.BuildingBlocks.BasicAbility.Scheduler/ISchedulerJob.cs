// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler;

public interface ISchedulerJob
{
    Task BeforeExcuteAsync(JobContext context);

    Task<object?> ExcuteAsync(JobContext context);

    Task AfterExcuteAsync(JobContext context);
}
