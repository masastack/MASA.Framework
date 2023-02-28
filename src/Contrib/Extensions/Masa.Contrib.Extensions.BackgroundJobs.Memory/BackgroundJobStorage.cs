// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory;

public class BackgroundJobStorage : IBackgroundJobStorage
{
    private readonly ConcurrentDictionary<Guid, BackgroundJobInfo> _jobs;

    public BackgroundJobStorage()
    {
        _jobs = new();
    }

    public Task InsertAsync(BackgroundJobInfo jobInfo)
    {
        _jobs[jobInfo.Id] = jobInfo;
        return Task.CompletedTask;
    }

    public Task<List<BackgroundJobInfo>> RetrieveJobsAsync(int batchSize)
    {
        var waitingJobs = _jobs.Values
            .Where(t => t.NextTryTime <= DateTime.UtcNow)
            .OrderBy(t => t.Times)
            .ThenBy(t => t.NextTryTime)
            .Take(batchSize)
            .ToList();

        return Task.FromResult(waitingJobs);
    }

    public Task DeleteAsync(Guid id)
    {
        _jobs.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(BackgroundJobInfo jobInfo)
    {
        if (jobInfo.IsInvalid)
            await DeleteAsync(jobInfo.Id);
    }
}
