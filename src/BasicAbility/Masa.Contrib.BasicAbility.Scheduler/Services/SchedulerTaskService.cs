// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Scheduler.Services;

public class SchedulerTaskService : ISchedulerTaskService
{
    const string API = "/api/scheduler-task";

    readonly ICallerProvider _callerProvider;
    readonly ILogger<SchedulerTaskService> _logger;
    readonly ILoggerFactory _loggerFactory;

    public SchedulerTaskService(ICallerProvider callerProvider, ILoggerFactory loggerFactory)
    {
        _callerProvider = callerProvider;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<SchedulerTaskService>();
    }

    public async Task<bool> StopAsync(BaseSchedulerTaskRequest request)
    {
        try
        {
            var requestUri = $"{API}/stop";
            await _callerProvider.PutAsync(requestUri, request);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StopSchedulerTaskAsync Error");
            return false;
        }
    }

    public async Task<bool> StartAsync(BaseSchedulerTaskRequest request)
    {
        try
        {
            var requestData = new StartSchedulerTaskRequest()
            {
                TaskId = request.TaskId,
                OperatorId = request.OperatorId,
                IsManual = true
            };

            var requestUri = $"{API}/start";
            await _callerProvider.PutAsync(requestUri, requestData);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "StopSchedulerTaskAsync Error");
            return false;
        }
    }
}
