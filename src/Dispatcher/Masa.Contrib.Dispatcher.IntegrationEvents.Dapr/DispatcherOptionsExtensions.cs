// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class DispatcherOptionsExtensions
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        string daprPubSubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(option => option.PubSubName = daprPubSubName);

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        Action<DispatcherOptions>? optionAction = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(optionAction, null);

    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        Action<DispatcherOptions>? optionAction,
        Action<DaprClientBuilder>? builder)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        ArgumentNullException.ThrowIfNull(dispatcherOptions.Services, nameof(dispatcherOptions.Services));

        dispatcherOptions.Services.TryAddDaprEventBus<TIntegrationEventLogService>(dispatcherOptions.Assemblies, option =>
        {
            option.PubSubName = DAPR_PUBSUB_NAME;
            optionAction?.Invoke(option);
        }, builder);
        return dispatcherOptions;
    }
}
