// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

public static class IntegrationEventOptionsExtensions
{
    public static Masa.Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions UseDapr(
        this Masa.Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions options,
        string daprPubSubName = DaprConstant.DAPR_PUBSUB_NAME,
        Action<DaprClientBuilder>? builder = null)
        => options.UseDapr(daprPubSubName, null, builder);

    public static Masa.Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions UseDapr(
        this Masa.Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions options,
        string daprPubSubName,
        string? daprAppId,
        Action<DaprClientBuilder>? builder = null)
    {
        options.Services.TryAddSingleton<IIntegrationEventDaprProvider, DefaultIntegrationEventDaprProvider>();
        options.Services.TryAddSingleton<IPublisher>(serviceProvider =>
            new Publisher(
                serviceProvider,
                daprPubSubName,
                serviceProvider.GetService<IOptions<MasaAppConfigureOptions>>()?.Value.AppId ?? string.Empty,
                serviceProvider.GetRequiredService<IIntegrationEventDaprProvider>().GetDaprAppId(daprAppId)));
        options.Services.AddDaprClient(builder);

        return options;
    }
}
