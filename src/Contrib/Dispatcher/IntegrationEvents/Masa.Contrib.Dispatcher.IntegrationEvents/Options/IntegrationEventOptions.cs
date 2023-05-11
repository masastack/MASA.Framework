// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Options;

#pragma warning disable S3236
public class IntegrationEventOptions : IIntegrationEventOptions
{
    public IServiceCollection Services { get; }

    public Assembly[] Assemblies { get; }

    private int _localRetryTimes = 3;

    /// <summary>
    /// Local queue maximum number of retries
    /// </summary>
    public int LocalRetryTimes
    {
        get => _localRetryTimes;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(LocalRetryTimes));

            _localRetryTimes = value;
        }
    }

    private int _maxRetryTimes = 10;

    /// <summary>
    /// Maximum number of retries
    /// Default is 10
    /// </summary>
    public int MaxRetryTimes
    {
        get => _maxRetryTimes;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(MaxRetryTimes));

            _maxRetryTimes = value;
        }
    }

    private int _failedRetryInterval = 60;

    /// <summary>
    /// The interval at which db polls for failure messages.
    /// Default is 60 seconds.
    /// Unit: seconds
    /// </summary>
    public int FailedRetryInterval
    {
        get => _failedRetryInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(FailedRetryInterval));

            _failedRetryInterval = value;
        }
    }

    private int _minimumRetryInterval = 60;

    /// <summary>
    /// Minimum execution retry interval
    /// Default is 60 seconds.
    /// </summary>
    public int MinimumRetryInterval
    {
        get => _minimumRetryInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(MinimumRetryInterval));

            _minimumRetryInterval = value;
        }
    }

    private int _localFailedRetryInterval = 3;

    /// <summary>
    /// The interval at which the local queue is polled for failed messages.
    /// Local queue does not rebuild after service crash
    /// Default is 3 seconds.
    /// Unit: seconds
    /// </summary>
    public int LocalFailedRetryInterval
    {
        get => _localFailedRetryInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(LocalFailedRetryInterval));

            _localFailedRetryInterval = value;
        }
    }

    private int _retryBatchSize = 100;

    /// <summary>
    /// The maximum number of retrieved messages per retry
    /// </summary>
    public int RetryBatchSize
    {
        get => _retryBatchSize;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(RetryBatchSize));

            _retryBatchSize = value;
        }
    }

    private int _batchSize = 20;

    /// <summary>
    /// The maximum retrieval status is the number of messages waiting to be sent each time
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

    private int _cleaningLocalQueueExpireInterval = 60;

    /// <summary>
    /// Delete local queue expired event interval
    /// Default is 60 seconds
    /// Unit: seconds
    /// </summary>
    public int CleaningLocalQueueExpireInterval
    {
        get => _cleaningLocalQueueExpireInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(CleaningLocalQueueExpireInterval));

            _cleaningLocalQueueExpireInterval = value;
        }
    }

    private int _cleaningExpireInterval = 300;

    /// <summary>
    /// Delete expired event interval
    /// Default is 300 seconds.
    /// Unit: seconds
    /// </summary>
    public int CleaningExpireInterval
    {
        get => _cleaningExpireInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(CleaningExpireInterval));

            _cleaningExpireInterval = value;
        }
    }

    private long _publishedExpireTime = 24 * 3600;

    /// <summary>
    /// Expiration time, when the message status is successful and has expired, it will be deleted by the scheduled task
    /// Default: ( 24 * 3600 )s
    /// </summary>
    public long PublishedExpireTime
    {
        get => _publishedExpireTime;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(PublishedExpireTime));

            _publishedExpireTime = value;
        }
    }

    private int _deleteBatchCount = 1000;

    /// <summary>
    /// Bulk delete expired messages
    /// </summary>
    public int DeleteBatchCount
    {
        get => _deleteBatchCount;
        set
        {
            MasaArgumentException.ThrowIfLessThanOrEqual(value, 0, nameof(DeleteBatchCount));

            _deleteBatchCount = value;
        }
    }

    public Func<DateTime>? GetCurrentTime { get; set; }

    private int _executeInterval = 1;

    /// <summary>
    /// The interval at which the execution sends integration events.
    /// Defaults to 1 second.
    /// Unit: second
    /// </summary>
    public int ExecuteInterval
    {
        get => _executeInterval;
        set
        {
            MasaArgumentException.ThrowIfLessThan(value, 1, nameof(DeleteBatchCount));

            _executeInterval = value;
        }
    }

    public List<Type> AllEventTypes { get; }

    private IntegrationEventOptions(IServiceCollection services) => Services = services;

    public IntegrationEventOptions(IServiceCollection services, Assembly[] assemblies)
        : this(services)
    {
        Assemblies = assemblies;
        AllEventTypes = assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && typeof(IEvent).IsAssignableFrom(type))
            .ToList();
    }
}
#pragma warning restore S3236
