// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller;

public class XmlRequestMessage : DefaultRequestMessage, IRequestMessage
{
    public XmlRequestMessage(
        IServiceProvider serviceProvider,
        IOptions<CallerFactoryOptions>? options = null)
        : base(serviceProvider, options)
    {
    }

    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        TrySetRequestId(requestMessage);
        TrySetCulture(requestMessage);
        return Task.FromResult(requestMessage);
    }

    public async Task<HttpRequestMessage> ProcessHttpRequestMessageAsync<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        requestMessage = await ProcessHttpRequestMessageAsync(requestMessage).ConfigureAwait(false);
        requestMessage.Content = new StringContent(XmlUtils.Serializer(data!));
        return requestMessage;
    }
}
