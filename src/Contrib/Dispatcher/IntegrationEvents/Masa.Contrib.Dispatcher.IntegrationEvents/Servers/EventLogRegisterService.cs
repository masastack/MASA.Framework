// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Servers;

internal class EventLogRegisterService : IProcessingServer
{
    private readonly SqlClientObserver _sqlClientObserver;

    public EventLogRegisterService(SqlClientObserver sqlClientObserver)
        => _sqlClientObserver = sqlClientObserver;

    public Task ExecuteAsync(CancellationToken stoppingToken)
    {
        DiagnosticListener.AllListeners.Subscribe(_sqlClientObserver);
        return Task.CompletedTask;
    }
}
