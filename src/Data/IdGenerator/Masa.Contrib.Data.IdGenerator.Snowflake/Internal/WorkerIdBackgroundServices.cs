﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

internal class WorkerIdBackgroundServices : BackgroundService
{
    private readonly int _heartbeatInterval;
    private readonly int _maxExpirationTime;
    private readonly IWorkerProvider _workerProvider;
    private readonly ILogger<WorkerIdBackgroundServices>? _logger;
    private bool _isAvailable = true;
    private DateTime? _lastFailedTime;

    public WorkerIdBackgroundServices(int heartbeatInterval, int maxExpirationTime, IWorkerProvider workerProvider,
        ILogger<WorkerIdBackgroundServices>? logger)
    {
        _heartbeatInterval = heartbeatInterval;
        _maxExpirationTime = maxExpirationTime;
        _workerProvider = workerProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            await _workerProvider.LogOutAsync();
        }
        else
        {
            while (true)
            {
                try
                {
                    if (_isAvailable)
                        await _workerProvider.RefreshAsync();
                    else
                        await _workerProvider.GetWorkerIdAsync(); //Get new WorkerId

                    _lastFailedTime = null;
                    _isAvailable = true;
                }
                catch (Exception ex)
                {
                    _lastFailedTime ??= DateTime.UtcNow;
                    if (_lastFailedTime != null && (DateTime.UtcNow - _lastFailedTime.Value).TotalMilliseconds > _maxExpirationTime)
                    {
                        _logger?.LogWarning(_isAvailable ?
                                "----- Logout WorkerId, Failed to refresh WorkerId, error reason: {Error}, current time: {CurrentTime}" :
                                "----- Logout WorkerId, Failed to get new WorkerId, error reason: {Error}, current time: {CurrentTime}",
                            ex,
                            DateTime.UtcNow);

                        await _workerProvider.LogOutAsync();
                    }
                    else
                    {
                        _logger?.LogWarning(
                            _isAvailable ? "Failed to refresh WorkerId, error reason: {Error}, current time: {CurrentTime}" :
                                "Failed to get new WorkerId, error reason: {Error}, current time: {CurrentTime}",
                            ex,
                            DateTime.UtcNow);
                    }

                    _isAvailable = false;
                }
                finally
                {
                    await Task.Delay(_heartbeatInterval, stoppingToken);
                }
            }
        }
    }
}
