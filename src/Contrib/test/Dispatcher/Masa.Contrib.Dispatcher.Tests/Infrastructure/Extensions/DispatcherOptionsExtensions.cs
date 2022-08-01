// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Dispatcher.Tests.Infrastructure.Extensions;

public static class DispatcherOptionsExtensions
{
    public static IDispatcherOptions UseTestPub(this IntegrationEvents.Options.DispatcherOptions dispatcherOptions)
    {
        dispatcherOptions.Services.TryAddSingleton<IPublisher, DefaultPublisher>();
        return dispatcherOptions;
    }
}
