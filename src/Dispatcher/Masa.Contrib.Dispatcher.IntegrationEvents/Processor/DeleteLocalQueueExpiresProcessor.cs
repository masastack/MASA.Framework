// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Processor;

public class DeleteLocalQueueExpiresProcessor : ProcessorBase
{
    private readonly IOptions<DispatcherOptions> _options;

    public override int Delay => _options.Value.CleaningLocalQueueExpireInterval;

    public DeleteLocalQueueExpiresProcessor(IOptions<DispatcherOptions> options) : base(null)
    {
        _options = options;
    }

    /// <summary>
    /// Delete expired events
    /// </summary>
    /// <returns></returns>
    protected override void Executing()
    {
        LocalQueueProcessor.Default.Delete(_options.Value.LocalRetryTimes);
    }
}
