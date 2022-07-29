// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public class JsonRequestMessage : DefaultRequestMessage, IRequestMessage
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public JsonRequestMessage(
        IServiceProvider serviceProvider,
        IOptions<CallerFactoryOptions>? options = null)
        : base(serviceProvider, options)
    {
        _jsonSerializerOptions = Options?.JsonSerializerOptions;
    }

    public virtual Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        TrySetRequestId(requestMessage);
        return Task.FromResult(requestMessage);
    }

    public virtual async Task<HttpRequestMessage> ProcessHttpRequestMessageAsync<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        requestMessage = await ProcessHttpRequestMessageAsync(requestMessage);
        requestMessage.Content = JsonContent.Create(data, options: _jsonSerializerOptions);
        return requestMessage;
    }
}
