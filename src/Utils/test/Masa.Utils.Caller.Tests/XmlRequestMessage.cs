// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Utils.Caller.Tests;

public class XmlRequestMessage : IRequestMessage
{
    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
        => Task.FromResult(requestMessage);

    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        var xmlContent = XmlUtils.Serializer(data!);
        requestMessage.Content = new StringContent(xmlContent);
        return Task.FromResult(requestMessage);
    }
}
