// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Internal;

internal class TransactionObserver : IObserver<KeyValuePair<string, object?>>
{
    private readonly LocalQueueEventLogService _localQueueEventLogService;

    private readonly ConcurrentDictionary<Guid, List<IntegrationEventLogItem>> _localEventLogs;

    public TransactionObserver(LocalQueueEventLogService localQueueEventLogService,
        ConcurrentDictionary<Guid, List<IntegrationEventLogItem>> localEventLogs)
    {
        _localQueueEventLogService = localQueueEventLogService;
        _localEventLogs = localEventLogs;
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(KeyValuePair<string, object?> value)
    {
        switch (value.Key)
        {

        }
    }
}
