// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public class MasaHttpContext
{
    private readonly Func<HttpRequestMessage>? _httpRequestMessageFunc;

    private HttpRequestMessage? _requestMessage;

    public HttpRequestMessage RequestMessage => _requestMessage ??= _httpRequestMessageFunc!.Invoke();

    public HttpResponseMessage ResponseMessage { get; internal set; }

    private readonly IResponseMessage _responseMessage;

    private MasaHttpContext(IResponseMessage responseMessage)
    {
        _responseMessage = responseMessage;
    }

    public MasaHttpContext(IResponseMessage responseMessage, HttpRequestMessage requestMessage)
        : this(responseMessage)
    {
        _requestMessage = requestMessage;
    }

    public MasaHttpContext(IResponseMessage responseMessage, Func<HttpRequestMessage>? httpRequestMessageFunc)
        : this(responseMessage)
    {
        if (httpRequestMessageFunc != null) _httpRequestMessageFunc = httpRequestMessageFunc;
        else _requestMessage = new HttpRequestMessage();
    }

    internal Task ProcessResponseAsync(CancellationToken cancellationToken = default)
        => _responseMessage.ProcessResponseAsync(ResponseMessage, cancellationToken);

    internal Task<TResponse?> ProcessResponseAsync<TResponse>(CancellationToken cancellationToken = default)
        => _responseMessage.ProcessResponseAsync<TResponse>(ResponseMessage, cancellationToken);
}
