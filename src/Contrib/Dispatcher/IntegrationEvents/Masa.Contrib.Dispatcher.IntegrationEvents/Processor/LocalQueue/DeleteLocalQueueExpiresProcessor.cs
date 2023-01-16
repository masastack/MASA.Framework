// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public class DeleteLocalQueueExpiresProcessor : ProcessorBase
{
    private readonly IOptions<IntegrationEventOptions> _options;

    public override int Delay => _options.Value.CleaningLocalQueueExpireInterval;

    public DeleteLocalQueueExpiresProcessor(
        IServiceProvider serviceProvider,
        IOptions<IntegrationEventOptions> options) : base(serviceProvider)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <returns></returns>
    protected override void Executing()
    {
        LocalQueueEventLogService.Delete(_options.Value.LocalRetryTimes);
    }
}
