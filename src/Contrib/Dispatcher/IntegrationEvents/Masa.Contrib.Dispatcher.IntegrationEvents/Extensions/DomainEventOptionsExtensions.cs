// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Dispatcher.IntegrationEvents;

[ExcludeFromCodeCoverage]
public static class DomainEventOptionsExtensions
{
    public static IDomainEventOptions UseIntegrationEventBus(
        this IDomainEventOptions options,
        Action<IntegrationEventOptions>? optionAction = null)
    {
        ArgumentNullException.ThrowIfNull(options.Services, nameof(options.Services));

        options.Services.TryAddIntegrationEventBus(options.Assemblies, option => optionAction?.Invoke(option));
        return options;
    }

    public static IDomainEventOptions UseIntegrationEventBus<TIntegrationEventLogService>(
        this IDomainEventOptions options,
        Action<IntegrationEventOptions>? optionAction = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        ArgumentNullException.ThrowIfNull(options.Services, nameof(options.Services));

        options.Services.TryAddIntegrationEventBus<TIntegrationEventLogService>(options.Assemblies,
            option => optionAction?.Invoke(option));
        return options;
    }
}
