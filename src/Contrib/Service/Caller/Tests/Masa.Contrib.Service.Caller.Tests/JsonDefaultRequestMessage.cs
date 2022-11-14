// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

public class JsonDefaultRequestMessage : DefaultRequestMessage
{
    public JsonDefaultRequestMessage(IServiceProvider serviceProvider, IOptions<CallerFactoryOptions>? options = null)
        : base(serviceProvider, options)
    {
    }

    public void TestTrySetCulture(HttpRequestMessage requestMessage, List<(string Key, string Value)> cultures)
    {
        base.TrySetCulture(requestMessage, cultures);
    }
}
