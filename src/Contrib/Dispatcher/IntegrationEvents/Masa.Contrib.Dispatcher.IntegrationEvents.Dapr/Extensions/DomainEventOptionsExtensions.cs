// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public static class DomainEventOptionsExtensions
{
    #region Obsolete

    [Obsolete("Use UseIntegrationEventBus<IntegrationEventLogService>(opt => opt.UseDapr())")]
    public static IDomainEventOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDomainEventOptions dispatcherOptions,
        string daprPubSubName)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(option => option.PubSubName = daprPubSubName);

    [Obsolete("Use UseIntegrationEventBus<IntegrationEventLogService>(opt => opt.UseDapr())")]
    public static IDomainEventOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDomainEventOptions dispatcherOptions,
        Action<DaprIntegrationEventOptions>? optionAction = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => dispatcherOptions.UseDaprEventBus<TIntegrationEventLogService>(optionAction, null);

    [Obsolete("Use UseIntegrationEventBus<IntegrationEventLogService>(opt => opt.UseDapr())")]
    public static IDomainEventOptions UseDaprEventBus<TIntegrationEventLogService>(
        this IDomainEventOptions dispatcherOptions,
        Action<DaprIntegrationEventOptions>? optionAction,
        Action<DaprClientBuilder>? builder)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        ArgumentNullException.ThrowIfNull(dispatcherOptions.Services, nameof(dispatcherOptions.Services));

        dispatcherOptions.Services.TryAddDaprEventBus<TIntegrationEventLogService>(dispatcherOptions.Assemblies, option =>
        {
            option.PubSubName = Constant.DAPR_PUBSUB_NAME;
            optionAction?.Invoke(option);
        }, builder);
        return dispatcherOptions;
    }

    #endregion
}
