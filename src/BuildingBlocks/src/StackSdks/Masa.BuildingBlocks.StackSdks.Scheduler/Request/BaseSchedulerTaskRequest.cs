// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

public class BaseSchedulerTaskRequest
{
    public Guid TaskId { get; set; }

    public Guid OperatorId { get; set; }
}
