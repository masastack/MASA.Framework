// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Internal;

internal class SqlClientObserver : IObserver<DiagnosticListener>
{
    private const string DIAGNOSTIC_LISTENER_NAME = "SqlClientDiagnosticListener";
    private readonly LocalQueueEventLogService _localQueueEventLogService;

    /// <summary>
    /// Transaction id and local message collection queue
    /// </summary>
    private ConcurrentDictionary<Guid, List<IntegrationEventLogItem>> LocalEventLogs { get; }

    public SqlClientObserver(LocalQueueEventLogService localQueueEventLogService)
    {
        _localQueueEventLogService = localQueueEventLogService;
        LocalEventLogs = new ConcurrentDictionary<Guid, List<IntegrationEventLogItem>>();
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
    }

    public void OnNext(DiagnosticListener value)
    {
        if (value.Name == DIAGNOSTIC_LISTENER_NAME)
            value.Subscribe(new TransactionObserver(_localQueueEventLogService, LocalEventLogs), IsEnabled);
    }

    private bool IsEnabled(string name)
    {
        return name == "System.Data.SqlClient.WriteCommandBefore";
    }
}
