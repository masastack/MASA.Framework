// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Scheduler.Enum;
using Masa.BuildingBlocks.StackSdks.Scheduler.Request;
using Masa.BuildingBlocks.StackSdks.Scheduler.Service;

namespace Masa.Contrib.StackSdks.Scheduler.Services;

public class SchedulerJobService : ISchedulerJobService
{
    const string API = "/api/scheduler-job";

    readonly ICaller _caller;
    readonly ILogger<SchedulerJobService>? _logger;

    public SchedulerJobService(ICaller caller, ILoggerFactory? loggerFactory = null)
    {
        _caller = caller;
        _logger = loggerFactory?.CreateLogger<SchedulerJobService>();
    }

    public async Task<Guid> AddAsync(AddSchedulerJobRequest request)
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

        try
        {
            var requestUri = $"{API}/addSchedulerJobBySdk";
            return await _caller.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, request);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "AddSchedulerJobAsync Error");
            return Guid.Empty;
        }

    }

    public async Task<bool> DisableAsync(BaseSchedulerJobRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logger?.LogError(ex, "DisableSchedulerJob Error");
            return false;
        }
    }

    public async Task<bool> EnableAsync(BaseSchedulerJobRequest request)
    {
        try
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
        catch (Exception ex)
        {
            _logger?.LogError(ex, "EnableSchedulerJob Error");
            return false;
        }
    }

    public async Task<bool> RemoveAsync(BaseSchedulerJobRequest request)
    {
        try
        {
            var requestUri = $"{API}";
            await _caller.DeleteAsync(requestUri, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "RemoveSchedulerJobAsync Error");
            return false;
        }
    }

    public async Task<bool> StartAsync(BaseSchedulerJobRequest request)
    {
        try
        {
            var requestUri = $"{API}/startJob";
            await _caller.PutAsync(requestUri, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "StartSchedulerJobAsync Error");
            return false;
        }
    }
}
