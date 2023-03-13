// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public class BackgroundJobProcessor : BackgroundJobProcessorBase
{
    private readonly IBackgroundJobExecutor _backgroundJobExecutor;
    public override int Period => _backgroundJobOptions.Value.PollInterval;

    private readonly IOptions<BackgroundJobOptions> _backgroundJobOptions;

    public BackgroundJobProcessor(
        IServiceProvider serviceProvider,
        IDeserializer deserializer)
        : base(serviceProvider, deserializer)
    {
        _backgroundJobExecutor = serviceProvider.GetRequiredService<IBackgroundJobExecutor>();
        _backgroundJobOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobOptions>>();
    }

    protected override async Task ExecuteJobAsync(CancellationToken cancellationToken)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        // The memory queue declaration cycle is a singleton
        var backgroundJobStorage = scope.ServiceProvider.GetRequiredService<IBackgroundJobStorage>();
        var jobs = await backgroundJobStorage.RetrieveJobsAsync(_backgroundJobOptions.Value.BatchSize);
        if (!jobs.Any())
            return;

        foreach (var job in jobs)
        {
            job.Times++;
            job.LastTryTime = DateTime.UtcNow;

            try
            {
                var jobArgs = GetJobArgs(job.Args, GetJobArgsType(job.Name));
                var jobContext = new JobContext(scope.ServiceProvider, GetJobTypeList(job.Name), jobArgs);
                try
                {
                    await _backgroundJobExecutor.ExecuteAsync(jobContext, cancellationToken);
                    await backgroundJobStorage.DeleteAsync(job.Id);
                }
                catch (Exception ex)
                {
                    Logger?.LogError(ex, "----- A background job execution is failed. Job: {JobInfo}", job);

                    await ExecuteFailedAsync(backgroundJobStorage, job);
                }
            }
            catch (BackgroundJobException ex)
            {
                Logger?.LogError(ex, "----- Error getting background task parameter information. Job: {JobInfo}", job);

                await ExecuteFailedAsync(backgroundJobStorage, job, false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "----- A background job execution is failed. Job: {JobInfo}", job);

                await ExecuteFailedAsync(backgroundJobStorage, job);
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

    private Task ExecuteFailedAsync(IBackgroundJobStorage backgroundJobStorage, BackgroundJobInfo jobInfo, bool isRetry = true)
    {
        if (isRetry)
        {
            var nextTryTime = NextTryTime(jobInfo);

            if (nextTryTime.HasValue)
            {
                jobInfo.NextTryTime = nextTryTime.Value;
            }
            else
            {
                jobInfo.IsInvalid = true;
            }
        }
        else
        {
            jobInfo.IsInvalid = true;
        }

        return backgroundJobStorage.UpdateAsync(jobInfo);
    }
}
