// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Masa.Contrib.Service.Caller.HttpClient;

internal static class MasaCallerClientBuilderExtensions
{
    public static IMasaCallerClientBuilder AddConfigHttpRequestMessage(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        Func<HttpRequestMessage, Task> httpRequestMessageFunc)
        => masaCallerClientBuilder.AddMiddleware(_ => new DefaultCallerMiddleware(httpRequestMessageFunc));
}
