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

    public async Task<Guid> AddAsync(UpsertSchedulerJobRequest request)
    {
        ValidateUpsertSchedulerJobRequest(request);

        var requestUri = $"{API}/addSchedulerJobBySdk";
        return await _caller.PostAsync<UpsertSchedulerJobRequest, Guid>(requestUri, request);
    }

    public async Task UpdateAsync(Guid id, UpsertSchedulerJobRequest request)
    {
        ValidateUpsertSchedulerJobRequest(request);

        var requestUri = $"{API}/{id}/updateSchedulerJobBySdk";
        await _caller.PutAsync<UpsertSchedulerJobRequest>(requestUri, request);
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

    private void ValidateUpsertSchedulerJobRequest(UpsertSchedulerJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProjectIdentity))
        {
            throw new ArgumentNullException(nameof(request.ProjectIdentity));
        }

        switch (request.JobType)
        {
            case JobTypes.JobApp:
                ArgumentNullException.ThrowIfNull(request.JobAppConfig, nameof(request.JobAppConfig));
                break;
            case JobTypes.Http:
                ArgumentNullException.ThrowIfNull(request.HttpConfig, nameof(request.HttpConfig));
                break;
            case JobTypes.DaprServiceInvocation:
                ArgumentNullException.ThrowIfNull(request.DaprServiceInvocationConfig, nameof(request.DaprServiceInvocationConfig));
                break;
        }
    }
}
