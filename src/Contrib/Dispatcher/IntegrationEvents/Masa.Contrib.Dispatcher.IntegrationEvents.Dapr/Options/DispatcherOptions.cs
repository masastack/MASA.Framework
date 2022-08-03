// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr.Options;

[Obsolete("Later versions will be changed to Internal")]
public class DispatcherOptions : Masa.Contrib.Dispatcher.IntegrationEvents.Options.DispatcherOptions
{
    private string _pubSubName = "pubsub";

    public string PubSubName
    {
        get => _pubSubName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(PubSubName));

            _pubSubName = value;
        }
    }

    public DispatcherOptions(IServiceCollection services, Assembly[] assemblies)
        : base(services, assemblies)
    {
    }

    [Obsolete("Later versions will be deleted")]
    internal void CopyTo(Masa.Contrib.Dispatcher.IntegrationEvents.Options.DispatcherOptions dispatcherOptions)
    {
        dispatcherOptions.LocalRetryTimes = LocalRetryTimes;
        dispatcherOptions.MaxRetryTimes = MaxRetryTimes;
        dispatcherOptions.FailedRetryInterval = FailedRetryInterval;
        dispatcherOptions.MinimumRetryInterval = MinimumRetryInterval;
        dispatcherOptions.LocalFailedRetryInterval = LocalFailedRetryInterval;
        dispatcherOptions.RetryBatchSize = RetryBatchSize;
        dispatcherOptions.CleaningLocalQueueExpireInterval = CleaningLocalQueueExpireInterval;
        dispatcherOptions.CleaningExpireInterval = CleaningExpireInterval;
        dispatcherOptions.PublishedExpireTime = PublishedExpireTime;
        dispatcherOptions.DeleteBatchCount = DeleteBatchCount;
        dispatcherOptions.GetCurrentTime = GetCurrentTime;
    }
}
