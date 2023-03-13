// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public class BackgroundJobOptions
{
    private int _pollInterval = 5;

    /// <summary>
    /// Background task rotation interval period
    /// Default: 5s
    /// </summary>
    public int PollInterval
    {
        get => _pollInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(PollInterval));

            _pollInterval = value;
        }
    }

    private int _batchSize = 30;

    /// <summary>
    /// Maximum number of retrieved messages per fetch
    /// </summary>
    public int BatchSize
    {
        get => _batchSize;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(BatchSize));

            _batchSize = value;
        }
    }

    private int _maxRetryTimes = 30;

    /// <summary>
    /// Maximum number of retries
    /// Default is 10
    /// </summary>
    public int MaxRetryTimes
    {
        get => _maxRetryTimes;
        set
        {
            MasaArgumentException.ThrowIfLessThan(value, 0, nameof(MaxRetryTimes));

            _maxRetryTimes = value;
        }
    }

    private int _firstWaitDuration = 60;

    /// <summary>
    /// Default: 60s
    /// </summary>
    public int FirstWaitDuration
    {
        get => _firstWaitDuration;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(FirstWaitDuration));

            _firstWaitDuration = value;
        }
    }

    private int _waitDuration = 2;

    /// <summary>
    /// Default: 2s
    /// </summary>
    public int WaitDuration
    {
        get => _waitDuration;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(WaitDuration));

            _waitDuration = value;
        }
    }
}
