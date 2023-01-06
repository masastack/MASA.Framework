// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.Tests;

public class CustomHttpClientCaller : HttpClientCaller
{
    public CustomHttpClientCaller(System.Net.Http.HttpClient httpClient, IServiceProvider serviceProvider, string baseApi)
        : base(httpClient, serviceProvider, baseApi)
    {
    }

    public string GetResult(string? methodName) => base.GetRequestUri(methodName);
}
