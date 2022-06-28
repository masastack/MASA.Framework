// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Scheduler.Services;

public class SchedulerJobService : ISchedulerJobService
{
    const string API = "/api/scheduler-job";

    readonly ICallerProvider _callerProvider;
    readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;
    readonly ILogger<SchedulerJobService> _logger;
    readonly ILoggerFactory _loggerFactory;

    public SchedulerJobService(ICallerProvider callerProvider, ILoggerFactory loggerFactory)
    {
        _callerProvider = callerProvider;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<SchedulerJobService>();
    }

    public async Task<Guid> AddbAsync(AddSchedulerJobRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProjectIdentity))
        {
            throw new ArgumentNullException(nameof(request.ProjectIdentity));
        }

        switch (request.JobType)
        {
            case BuildingBlocks.BasicAbility.Scheduler.Enum.JobTypes.JobApp:
                ArgumentNullException.ThrowIfNull(request.JobAppConfig, nameof(request.JobAppConfig));
                break;
            case BuildingBlocks.BasicAbility.Scheduler.Enum.JobTypes.Http:
                ArgumentNullException.ThrowIfNull(request.HttpConfig, nameof(request.HttpConfig));
                break;
            case BuildingBlocks.BasicAbility.Scheduler.Enum.JobTypes.DaprServiceInvocation:
                ArgumentNullException.ThrowIfNull(request.DaprServiceInvocationConfig, nameof(request.DaprServiceInvocationConfig));
                break;
        }

        try
        {
            var requestUri = $"{API}/addSchedulerJobBySdk";
            return await _callerProvider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AddSchedulerJobAsync Error");
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
            await _callerProvider.PutAsync<ChangeEnabledStatusRequest>(requestUri, requestData);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DisableSchedulerJob Error");
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
            await _callerProvider.PutAsync<ChangeEnabledStatusRequest>(requestUri, requestData);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "EnableSchedulerJob Error");
            return false;
        }
    }

    public async Task<bool> RemoveAsync(BaseSchedulerJobRequest request)
    {
        try
        {
            var requestUri = $"{API}";
            await _callerProvider.DeleteAsync(requestUri, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RemoveSchedulerJobAsync Error");
            return false;
        }
    }

    public async Task<bool> StartAsync(BaseSchedulerJobRequest request)
    {
        try
        {
            var requestUri = $"{API}/startJob";
            await _callerProvider.PutAsync(requestUri, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StartSchedulerJobAsync Error");
            return false;
        }
    }
}
