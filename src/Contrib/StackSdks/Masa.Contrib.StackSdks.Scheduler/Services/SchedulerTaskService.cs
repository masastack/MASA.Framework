// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler.Services;

public class SchedulerTaskService : ISchedulerTaskService
{
    const string API = "/api/scheduler-task";

    readonly ICaller _caller;

    public SchedulerTaskService(ICaller caller)
    {
        _caller = caller;
    }

    public async Task<bool> StopAsync(SchedulerTaskRequestBase request)
    {
        var requestUri = $"{API}/stop";
        await _caller.PutAsync(requestUri, request);
        return true;
    }

    public async Task<bool> StartAsync(SchedulerTaskRequestBase request)
    {
        var requestData = new StartSchedulerTaskRequest()
        {
            TaskId = request.TaskId,
            OperatorId = request.OperatorId,
            IsManual = true
        };

        var requestUri = $"{API}/start";
        await _caller.PutAsync(requestUri, requestData);
        return true;
    }
}
