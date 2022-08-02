// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Dispatcher.Events;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }

    IEventBusBuilder UseMiddleware(Type middleware, ServiceLifetime middlewareLifetime = ServiceLifetime.Transient);
}
