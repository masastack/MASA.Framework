// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public class XmlRequestMessage : DefaultRequestMessage, IRequestMessage
{
    public XmlRequestMessage(
        IOptions<CallerFactoryOptions> options,
        IRequestIdGenerator requestIdGenerator,
        IHttpContextAccessor? httpContextAccessor = null)
        : base(options, requestIdGenerator, httpContextAccessor)
    {
    }

    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        TrySetRequestId(requestMessage);
        return Task.FromResult(requestMessage);
    }

    public async Task<HttpRequestMessage> ProcessHttpRequestMessageAsync<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        requestMessage = await ProcessHttpRequestMessageAsync(requestMessage);
        requestMessage.Content = new StringContent(XmlUtils.Serializer(data!));
        return requestMessage;
    }
}
