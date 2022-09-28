// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

[Obsolete("BaseSchedulerTaskRequest has expired, please use SchedulerTaskRequestBase")]
public class BaseSchedulerTaskRequest : SchedulerTaskRequestBase
{
}

public class SchedulerTaskRequestBase
{
    public Guid TaskId { get; set; }

    public Guid OperatorId { get; set; }
}
