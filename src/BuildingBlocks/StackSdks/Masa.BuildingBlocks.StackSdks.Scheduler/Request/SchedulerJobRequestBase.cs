// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Request;

[Obsolete("BaseSchedulerJobRequest has expired, please use SchedulerJobRequestBase")]
public class BaseSchedulerJobRequest : SchedulerJobRequestBase
{

}

public class SchedulerJobRequestBase
{
    public Guid JobId { get; set; }

    public Guid OperatorId { get; set; }
}
