﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Serialization.Xml;

public class XmlRequestMessage : IRequestMessage
{
    public virtual void ProcessHttpRequestMessage(HttpRequestMessage requestMessage)
    {
    }

    public virtual void ProcessHttpRequestMessage<TRequest>(HttpRequestMessage requestMessage, TRequest data)
    {
        ProcessHttpRequestMessage(requestMessage);
        requestMessage.Content = new StringContent(XmlUtils.Serializer(data!));
    }
}
