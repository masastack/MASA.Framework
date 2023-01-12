// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class DefaultCallerMiddleware : ICallerMiddleware
{
    private readonly Func<IServiceProvider, HttpRequestMessage, Task> _httpRequestMessageFunc;

    public DefaultCallerMiddleware(Func<IServiceProvider, HttpRequestMessage, Task> httpRequestMessageFunc)
    {
        _httpRequestMessageFunc = httpRequestMessageFunc;
    }

    public Task HandleAsync(MasaHttpContext masaHttpContext, CallerHandlerDelegate next, CancellationToken cancellationToken = default)
    {
        _httpRequestMessageFunc.Invoke(masaHttpContext.ServiceProvider, masaHttpContext.RequestMessage);
        return next();
    }
}
