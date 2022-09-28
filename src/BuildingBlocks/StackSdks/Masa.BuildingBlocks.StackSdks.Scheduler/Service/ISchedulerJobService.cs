// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Scheduler.Service;

public interface ISchedulerJobService
{
    Task<Guid> AddAsync(AddSchedulerJobRequest job);

    Task<bool> StartAsync(SchedulerJobRequestBase request);

    Task<bool> RemoveAsync(SchedulerJobRequestBase request);

    Task<bool> EnableAsync(SchedulerJobRequestBase request);

    Task<bool> DisableAsync(SchedulerJobRequestBase request);

    Task<SchedulerJobModel?> GetSchedulerJobQueryByIdentityAsync(GetSchedulerJobByIdentityRequest request);
}
