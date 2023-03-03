// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Hangfire;

[ExcludeFromCodeCoverage]
public class DefaultBackgroundJobManager : IBackgroundJobManager
{
    private readonly IServiceProvider _serviceProvider;

    public DefaultBackgroundJobManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<string> EnqueueAsync<TArgs>(TArgs args, TimeSpan? delay = null)
    {
        return Task.FromResult(delay.HasValue ?
            BackgroundJob.Schedule<BackgroundJobExecutorAdapter<TArgs>>(adapter => adapter.ExecuteAsync(args), delay.Value) :
            BackgroundJob.Enqueue<BackgroundJobExecutorAdapter<TArgs>>(adapter => adapter.ExecuteAsync(args))
        );
    }

    public Task AddOrUpdateScheduleAsync(IBackgroundScheduleJob backgroundScheduleJob)
    {
        if (backgroundScheduleJob is IHangfireBackgroundScheduleJob hangfireBackgroundScheduleJob)
        {
            if (backgroundScheduleJob.Id.IsNullOrWhiteSpace())
            {
                RecurringJob.AddOrUpdate(
                    () => hangfireBackgroundScheduleJob.ExecuteAsync(_serviceProvider.CreateScope().ServiceProvider),
                    hangfireBackgroundScheduleJob.CronExpression,
                    hangfireBackgroundScheduleJob.TimeZone,
                    hangfireBackgroundScheduleJob.Queue);
            }
            else
            {
                RecurringJob.AddOrUpdate(
                    hangfireBackgroundScheduleJob.Id,
                    () => hangfireBackgroundScheduleJob.ExecuteAsync(_serviceProvider.CreateScope().ServiceProvider),
                    hangfireBackgroundScheduleJob.CronExpression,
                    hangfireBackgroundScheduleJob.TimeZone,
                    hangfireBackgroundScheduleJob.Queue);
            }
            return Task.CompletedTask;
        }

        throw new BackgroundJobException(errorCode: ExceptionErrorCode.NOT_SUPPORT_PERIODICALLY_TASK_TYPE_JOB);
    }
}
