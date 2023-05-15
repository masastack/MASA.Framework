// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler.Services;

public class SchedulerJobService : ISchedulerJobService
{
    const string API = "/api/scheduler-job";

    readonly ICaller _caller;

    public SchedulerJobService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<Guid> AddAsync(UpsertSchedulerJobRequest job)
    {
        ValidateUpsertSchedulerJobRequest(job);

        var requestUri = $"{API}/addSchedulerJobBySdk";
        return await _caller.PostAsync<UpsertSchedulerJobRequest, Guid>(requestUri, job);
    }

    public async Task UpdateAsync(Guid id, UpsertSchedulerJobRequest job)
    {
        ValidateUpsertSchedulerJobRequest(job);

        var requestUri = $"{API}/{id}/updateSchedulerJobBySdk";
        await _caller.PutAsync<UpsertSchedulerJobRequest>(requestUri, job);
    }

    public async Task<bool> DisableAsync(SchedulerJobRequestBase request)
    {
        var requestData = new ChangeEnabledStatusRequest()
        {
            JobId = request.JobId,
            OperatorId = request.OperatorId,
            Enabled = false
        };
        var requestUri = $"{API}/changeEnableStatus";
        await _caller.PutAsync<ChangeEnabledStatusRequest>(requestUri, requestData);
        return true;
    }

    public async Task<bool> EnableAsync(SchedulerJobRequestBase request)
    {
        var requestData = new ChangeEnabledStatusRequest()
        {
            JobId = request.JobId,
            OperatorId = request.OperatorId,
            Enabled = true
        };
        var requestUri = $"{API}/changeEnableStatus";
        await _caller.PutAsync<ChangeEnabledStatusRequest>(requestUri, requestData);
        return true;
    }

    public async Task<SchedulerJobModel?> GetSchedulerJobQueryByIdentityAsync(GetSchedulerJobByIdentityRequest request)
    {
        var requestUri = $"{API}/getSchedulerJobQueryByIdentity";
        return await _caller.GetAsync<SchedulerJobModel?>(requestUri, request);
    }

    public async Task<bool> RemoveAsync(SchedulerJobRequestBase request)
    {
        var requestUri = $"{API}";
        await _caller.DeleteAsync(requestUri, request);
        return true;
    }

    public async Task<bool> StartAsync(SchedulerJobRequestBase request)
    {
        var requestUri = $"{API}/startJob";
        await _caller.PutAsync(requestUri, request);
        return true;
    }

    private static void ValidateUpsertSchedulerJobRequest(UpsertSchedulerJobRequest job)
    {
        if (string.IsNullOrWhiteSpace(job.ProjectIdentity))
        {
            throw new ArgumentNullException(nameof(UpsertSchedulerJobRequest.ProjectIdentity));
        }

        switch (job.JobType)
        {
            case JobTypes.JobApp:
                ArgumentNullException.ThrowIfNull(job.JobAppConfig, nameof(job.JobAppConfig));
                break;
            case JobTypes.Http:
                ArgumentNullException.ThrowIfNull(job.HttpConfig, nameof(job.HttpConfig));
                break;
            case JobTypes.DaprServiceInvocation:
                ArgumentNullException.ThrowIfNull(job.DaprServiceInvocationConfig, nameof(job.DaprServiceInvocationConfig));
                break;
        }
    }
}
