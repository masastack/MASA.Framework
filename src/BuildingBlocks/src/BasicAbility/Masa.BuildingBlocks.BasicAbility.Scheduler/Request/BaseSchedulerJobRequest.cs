// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler.Request;

public class BaseSchedulerJobRequest
{
    public Guid JobId { get; set; }

    public Guid OperatorId { get; set; }
}
