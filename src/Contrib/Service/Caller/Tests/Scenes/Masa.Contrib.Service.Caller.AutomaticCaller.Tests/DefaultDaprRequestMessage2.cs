// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests;

public class DefaultDaprRequestMessage2 : IDaprRequestMessage
{
    private readonly List<(string Name, string Value)> _headers;

    public DefaultDaprRequestMessage2(List<(string Name, string Value)> headers)
    {
        _headers = headers;
    }

    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        foreach (var header in _headers)
        {
            requestMessage.Headers.Add(header.Name, header.Value);
        }
        return Task.FromResult(requestMessage);
    }
}
