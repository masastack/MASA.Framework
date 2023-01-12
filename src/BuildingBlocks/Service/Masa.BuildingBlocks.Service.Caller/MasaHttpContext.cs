// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public class MasaHttpContext
{
    private readonly Func<HttpRequestMessage>? _httpRequestMessageFunc;

    private HttpRequestMessage? _requestMessage;

    public HttpRequestMessage RequestMessage => _requestMessage ??= _httpRequestMessageFunc!.Invoke();

    public HttpResponseMessage ResponseMessage { get; internal set; }

    public IServiceProvider ServiceProvider { get; }

    private readonly IResponseMessage _responseMessage;

    private MasaHttpContext(IServiceProvider serviceProvider, IResponseMessage responseMessage)
    {
        ServiceProvider = serviceProvider;
        _responseMessage = responseMessage;
    }

    public MasaHttpContext(IServiceProvider serviceProvider, IResponseMessage responseMessage, HttpRequestMessage requestMessage)
        : this(serviceProvider, responseMessage)
    {
        ServiceProvider = serviceProvider;
        _requestMessage = requestMessage;
    }

    public MasaHttpContext(IServiceProvider serviceProvider, IResponseMessage responseMessage, Func<HttpRequestMessage>? httpRequestMessageFunc)
        : this(serviceProvider, responseMessage)
    {
        if (httpRequestMessageFunc != null) _httpRequestMessageFunc = httpRequestMessageFunc;
        else _requestMessage = new HttpRequestMessage();
    }

    internal Task ProcessResponseAsync(CancellationToken cancellationToken = default)
        => _responseMessage.ProcessResponseAsync(ResponseMessage, cancellationToken);

    internal Task<TResponse?> ProcessResponseAsync<TResponse>(CancellationToken cancellationToken = default)
        => _responseMessage.ProcessResponseAsync<TResponse>(ResponseMessage, cancellationToken);
}
