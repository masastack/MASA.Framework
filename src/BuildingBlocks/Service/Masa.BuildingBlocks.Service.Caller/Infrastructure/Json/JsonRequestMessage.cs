// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public class JsonRequestMessage : IRequestMessage
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions;

    public JsonRequestMessage(JsonSerializerOptions? jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions ?? MasaApp.GetJsonSerializerOptions();
    }

    public virtual void ProcessHttpRequestMessage(HttpRequestMessage requestMessage)
    {
    }

    public virtual void ProcessHttpRequestMessage<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        ProcessHttpRequestMessage(requestMessage);
        requestMessage.Content = JsonContent.Create(data, options: _jsonSerializerOptions);
    }
}
