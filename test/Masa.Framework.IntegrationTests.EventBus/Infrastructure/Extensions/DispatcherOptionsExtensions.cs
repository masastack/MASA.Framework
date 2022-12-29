// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Framework.IntegrationTests.EventBus.Infrastructure.Extensions;

public static class DispatcherOptionsExtensions
{
    public static Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions UseTestPub(
        this Contrib.Dispatcher.IntegrationEvents.Options.IntegrationEventOptions dispatcherOptions)
    {
        dispatcherOptions.Services.TryAddSingleton<IPublisher, DefaultPublisher>();
        return dispatcherOptions;
    }
}
