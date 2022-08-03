// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.DaprClient.Options;

public class CallerDaprClientOptions
{
    public IList<Action<MasaHttpMessageHandlerBuilder>> HttpRequestMessageActions { get; } = new List<Action<MasaHttpMessageHandlerBuilder>>();
}
