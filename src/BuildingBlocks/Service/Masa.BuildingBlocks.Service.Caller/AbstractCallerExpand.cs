// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public abstract class AbstractCallerExpand : AbstractCaller, ICallerExpand
{
    private Func<HttpRequestMessage, Task>? _requestMessageFunc;

    protected AbstractCallerExpand(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected AbstractCallerExpand(
        IServiceProvider serviceProvider,
        string name,
        Func<IServiceProvider, IRequestMessage>? currentRequestMessageFactory,
        Func<IServiceProvider, IResponseMessage>? currentResponseMessageFactory)
        : base(serviceProvider, name, currentRequestMessageFactory, currentResponseMessageFactory)
    {
    }

    public void ConfigRequestMessage(Func<HttpRequestMessage, Task> func)
        => _requestMessageFunc = func;

    protected override async Task<HttpResponseMessage> SendAsync(
        Func<HttpRequestMessage> requestMessageFunc,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        var masaHttpContext = new MasaHttpContext(ResponseMessage, requestMessageFunc);
        _requestMessageFunc?.Invoke(masaHttpContext.RequestMessage);

        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);
        };

        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return masaHttpContext.ResponseMessage;
    }

    protected override async Task<TResponse?> SendAsync<TResponse>(Func<HttpRequestMessage> requestMessageFunc, CancellationToken cancellationToken = default)
        where TResponse : default
    {
        TResponse? response = default;
        var masaHttpContext = new MasaHttpContext(ResponseMessage, requestMessageFunc);
        _requestMessageFunc?.Invoke(masaHttpContext.RequestMessage);

        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);
            response = await masaHttpContext.ProcessResponseAsync<TResponse>(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return response;
    }

    public override async Task<string> GetStringAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        string content = default!;
        var masaHttpContext = new MasaHttpContext(ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        _requestMessageFunc?.Invoke(masaHttpContext.RequestMessage);

        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        };

        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }

    public override async Task<byte[]> GetByteArrayAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        byte[] content = default!;
        var masaHttpContext = new MasaHttpContext(ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        _requestMessageFunc?.Invoke(masaHttpContext.RequestMessage);

        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsByteArrayAsync(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }

    public override async Task<Stream> GetStreamAsync(
        string? methodName,
        bool autoThrowException = true,
        CancellationToken cancellationToken = default)
    {
        Stream content = default!;
        var masaHttpContext = new MasaHttpContext(ResponseMessage, () => CreateRequest(HttpMethod.Get, methodName));
        _requestMessageFunc?.Invoke(masaHttpContext.RequestMessage);

        CallerHandlerDelegate callerHandlerDelegate = async () =>
        {
            masaHttpContext.ResponseMessage = await SendAsync(masaHttpContext.RequestMessage, cancellationToken);

            if (autoThrowException) await masaHttpContext.ProcessResponseAsync(cancellationToken);

            content = await masaHttpContext.ResponseMessage.Content.ReadAsStreamAsync(cancellationToken);
        };
        await Middlewares.Aggregate(callerHandlerDelegate,
            (next, func) => () => func.Invoke(ServiceProvider).HandleAsync(masaHttpContext, next, cancellationToken))();
        return content;
    }
}
