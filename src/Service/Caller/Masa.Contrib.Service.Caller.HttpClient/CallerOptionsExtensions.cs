// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public static class CallerOptionsExtensions
{
    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions, Func<MasaHttpClientBuilder>? clientBuilder = null)
    {
        var builder = clientBuilder == null ? new MasaHttpClientBuilder() : clientBuilder.Invoke();
        var httpClientBuilder = callerOptions.Services.AddHttpClient(builder.Name, httpClient
            => builder.ConfigureHttpClient(httpClient));

        AddCallerExtensions.AddCaller(callerOptions, builder.Name, builder.IsDefault, serviceProvider
            => new HttpClientCaller(serviceProvider, builder.Name, builder.Prefix));
        return httpClientBuilder;
    }

    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions, Action<MasaHttpClientBuilder>? clientBuilder)
    {
        MasaHttpClientBuilder builder = new MasaHttpClientBuilder();
        clientBuilder?.Invoke(builder);

        return callerOptions.UseHttpClient(() => builder);
    }
}
