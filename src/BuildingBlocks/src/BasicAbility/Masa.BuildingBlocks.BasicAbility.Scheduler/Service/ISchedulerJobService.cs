// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.BasicAbility.Scheduler.Service;

public interface ISchedulerJobService
{
    Task<Guid> AddAsync(AddSchedulerJobRequest job);

    Task<bool> StartAsync(BaseSchedulerJobRequest request);

    Task<bool> RemoveAsync(BaseSchedulerJobRequest request);

    Task<bool> EnableAsync(BaseSchedulerJobRequest request);

    Task<bool> DisableAsync(BaseSchedulerJobRequest request);
}
