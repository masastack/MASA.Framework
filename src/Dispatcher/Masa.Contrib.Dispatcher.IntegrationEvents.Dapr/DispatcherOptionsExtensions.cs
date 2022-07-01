// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Dapr;

public static class DispatcherOptionsExtensions
{
    private const string DAPR_PUBSUB_NAME = "pubsub";

    public static IDispatcherOptions UseDapr(
        this Masa.Contrib.Dispatcher.IntegrationEvents.Options.DispatcherOptions dispatcherOptions,
        string daprPubSubName = DAPR_PUBSUB_NAME,
        Action<DaprClientBuilder>? builder = null)
    {
        dispatcherOptions.Services.TryAddSingleton<IPublisher>(serviceProvider => new Publisher(serviceProvider, daprPubSubName));
        dispatcherOptions.Services.AddDaprClient(builder);
        return dispatcherOptions;
    }

    #region Obsolete

    [Obsolete("Use UseDapr instead")]
    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        string daprPubSubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(option => option.PubSubName = daprPubSubName);


    [Obsolete("Use UseDapr instead")]
    public static IDistributedDispatcherOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        Action<DispatcherOptions>? optionAction = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(optionAction, null);

    [Obsolete("Use UseDapr instead")]
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

    #endregion

}
