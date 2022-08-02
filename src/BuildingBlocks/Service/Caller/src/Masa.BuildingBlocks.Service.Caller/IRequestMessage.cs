// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Service.Caller;

public interface IRequestMessage
{
    Task<HttpRequestMessage> ProcessHttpRequestMessageAsync(HttpRequestMessage requestMessage);

    Task<HttpRequestMessage> ProcessHttpRequestMessageAsync<TRequest>(HttpRequestMessage requestMessage, TRequest data);
}
