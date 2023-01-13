// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Serialization.Xml.Tests;

public class CustomXmlResponseMessage : XmlResponseMessage
{
    public Task<TResponse?> GetCustomResponseAsync<TResponse>(
        HttpContent httpContent,
        CancellationToken cancellationToken = default)
        => base.FormatResponseAsync<TResponse>(httpContent, cancellationToken);
}
