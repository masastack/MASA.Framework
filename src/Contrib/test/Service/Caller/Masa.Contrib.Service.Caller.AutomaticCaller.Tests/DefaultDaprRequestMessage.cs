// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.AutomaticCaller.Tests;

public class DefaultDaprRequestMessage : IDaprRequestMessage
{
    public Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage)
    {
        requestMessage.Headers.Add("test", "test");
        return Task.FromResult(requestMessage);
    }
}
