// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

public class NotifySchedulerTaskRunResultRequest: SchedulerTaskRequestBase
{
    public TaskRunResultStatus Status { get; set; }

    public string? Message { get; set; }
}
