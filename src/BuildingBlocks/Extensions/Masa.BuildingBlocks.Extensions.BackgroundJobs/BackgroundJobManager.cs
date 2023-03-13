// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Hangfire.Tests")]
namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class BackgroundJobManager
{
    private static IBackgroundJobManager? _backgroundJobManager;

    private static IBackgroundJobManager JobManager => _backgroundJobManager ??= MasaApp.GetRequiredService<IBackgroundJobManager>();

    /// <summary>
    /// Execute only one time
    /// </summary>
    /// <param name="args"></param>
    /// <param name="delay"></param>
    /// <typeparam name="TArgs"></typeparam>
    /// <returns></returns>
    public static Task<string> EnqueueAsync<TArgs>(TArgs args, TimeSpan? delay = null)
        => JobManager.EnqueueAsync(args, delay);

    /// <summary>
    /// Add recurring job tasks
    /// </summary>
    /// <param name="backgroundScheduleJob"></param>
    /// <returns></returns>
    public static Task AddOrUpdateScheduleAsync(IBackgroundScheduleJob backgroundScheduleJob)
        => JobManager.AddOrUpdateScheduleAsync(backgroundScheduleJob);

    internal static void ResetBackgroundJobManager()
    {
        _backgroundJobManager = null;
    }
}
