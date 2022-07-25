// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

public class CustomHttpClientCallerProvider : HttpClientCallerProvider
{
    public CustomHttpClientCallerProvider(IServiceProvider serviceProvider, string name, string baseApi)
        : base(serviceProvider, name, baseApi)
    {
    }

    public string GetResult(string? methodName) => base.GetRequestUri(methodName);
}
