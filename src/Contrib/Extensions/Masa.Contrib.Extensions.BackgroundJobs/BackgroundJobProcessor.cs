// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public class BackgroundJobProcessor : BackgroundJobProcessorBase
{
    private readonly IOptions<BackgroundJobOptions> _backgroundJobOptions;

    public BackgroundJobProcessor(
        IServiceProvider serviceProvider,
        IDeserializer deserializer)
        : base(serviceProvider, deserializer)
    {
        _backgroundJobOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobOptions>>();
    }

    protected override async Task ExecuteJobAsync(BackgroundJobContext backgroundJobContext, CancellationToken cancellationToken)
    {
        var backgroundJobStorage = backgroundJobContext.ServiceProvider.GetRequiredService<IBackgroundJobStorage>();
        var jobs = await backgroundJobStorage.RetrieveJobsAsync(_backgroundJobOptions.Value.BatchSize);
        if (!jobs.Any())
            return;

        var jobExecutor = backgroundJobContext.ServiceProvider.GetRequiredService<IBackgroundJobExecutor>();

        foreach (var job in jobs)
        {
            job.Times++;
            job.LastTryTime = DateTime.UtcNow;

            try
            {
                var jobArgs = GetJobArgs(job.Args, GetJobArgsType(job.Name));
                var jobContext = new JobContext(backgroundJobContext.ServiceProvider, GetJobTypeList(job.Name), jobArgs);
                try
                {
                    await jobExecutor.ExecuteAsync(jobContext, cancellationToken);
                    await backgroundJobStorage.DeleteAsync(job.Id);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "----- A background job execution is failed. Job: {JobInfo}", job);

                    var nextTryTime = NextTryTime(job);

                    if (nextTryTime.HasValue)
                    {
                        job.NextTryTime = nextTryTime.Value;
                    }
                    else
                    {
                        job.IsInvalid = true;
                    }
                    await backgroundJobStorage.UpdateAsync(job);
                }
            }
            catch (BackgroundJobException ex)
            {
                Logger?.LogError(ex, "----- Error getting background task parameter information. Job: {JobInfo}", job);

                var nextTryTime = NextTryTime(job);

                if (nextTryTime.HasValue)
                {
                    job.NextTryTime = nextTryTime.Value;
                }
                else
                {
                    job.IsInvalid = true;
                }

                await backgroundJobStorage.UpdateAsync(job);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "----- A background job execution is failed. Job: {JobInfo}", job);

                var nextTryTime = NextTryTime(job);

                if (nextTryTime.HasValue)
                {
                    job.NextTryTime = nextTryTime.Value;
                }
                else
                {
                    job.IsInvalid = true;
                }

                await backgroundJobStorage.UpdateAsync(job);
            }
        }
    }

    protected virtual DateTime? NextTryTime(BackgroundJobInfo jobInfo)
    {
        if (jobInfo.Times >= _backgroundJobOptions.Value.MaxRetryTimes + 1)
            return null;

        var nextDuration = _backgroundJobOptions.Value.FirstWaitDuration *
            Math.Pow(_backgroundJobOptions.Value.WaitDuration, jobInfo.Times - 1);
        return jobInfo.LastTryTime.AddSeconds(nextDuration);
    }
}
