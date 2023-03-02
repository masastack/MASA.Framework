// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public interface IBackgroundJobManager
{
    /// <summary>
    /// Execute only one time
    /// </summary>
    /// <param name="args"></param>
    /// <param name="delay"></param>
    /// <typeparam name="TArgs"></typeparam>
    /// <returns></returns>
    Task<string> EnqueueAsync<TArgs>(TArgs args, TimeSpan? delay = null);

    /// <summary>
    /// Add recurring job tasks
    /// </summary>
    /// <param name="backgroundScheduleJob"></param>
    /// <returns></returns>
    Task<string> AddOrUpdateScheduleAsync(IBackgroundScheduleJob backgroundScheduleJob);
}
