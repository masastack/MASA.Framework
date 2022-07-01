// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.IntegrationEvents;

public static class DispatcherOptionsExtensions
{
    public static IDistributedDispatcherOptions UseIntegrationEventBus<TIntegrationEventLogService>(
        this IDistributedDispatcherOptions dispatcherOptions,
        Action<DispatcherOptions>? optionAction = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
    {
        ArgumentNullException.ThrowIfNull(dispatcherOptions.Services, nameof(dispatcherOptions.Services));

        dispatcherOptions.Services.TryAddIntegrationEventBus<TIntegrationEventLogService>(dispatcherOptions.Assemblies, option =>
        {
            optionAction?.Invoke(option);
        });
        return dispatcherOptions;
    }
}
