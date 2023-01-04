// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Service.Caller.HttpClient;

public static class CallerOptionsExtensions
{
    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        Func<MasaHttpClientBuilder>? clientBuilder = null,
        bool isSupportUpdate = false)
        => callerOptions.UseHttpClient(Microsoft.Extensions.Options.Options.DefaultName, clientBuilder, isSupportUpdate);

    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        string name,
        Func<MasaHttpClientBuilder>? clientBuilder = null,
        bool isSupportUpdate = false)
    {
        return callerOptions.UseHttpClient(name, masaHttpClientBuilder =>
        {
            var builder = clientBuilder == null ? new MasaHttpClientBuilder() : clientBuilder.Invoke();
            masaHttpClientBuilder.BaseAddress = builder.BaseAddress;
            masaHttpClientBuilder.Prefix = builder.Prefix;
            masaHttpClientBuilder.Configure = builder.Configure;
        }, isSupportUpdate);
    }

    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        Action<MasaHttpClientBuilder>? clientBuilder,
        bool isSupportUpdate = false)
        => callerOptions.UseHttpClient(Microsoft.Extensions.Options.Options.DefaultName, clientBuilder, isSupportUpdate);

    public static IHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        string name,
        Action<MasaHttpClientBuilder>? clientBuilder,
        bool isSupportUpdate = false)
    {
        if (isSupportUpdate)
        {
            var httpClientBuilder = callerOptions.Services.AddHttpClient(name);
            AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider
            =>
            {
                var masaHttpClientBuilder = new MasaHttpClientBuilder();
                clientBuilder?.Invoke(masaHttpClientBuilder);
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
                masaHttpClientBuilder.ConfigureHttpClient(httpClient);
                return new HttpClientCaller(httpClient, serviceProvider, masaHttpClientBuilder.Prefix);
            });
            return httpClientBuilder;
        }
        else
        {
            var masaHttpClientBuilder = new MasaHttpClientBuilder();
            clientBuilder?.Invoke(masaHttpClientBuilder);
            var httpClientBuilder = callerOptions.Services.AddHttpClient(name, httpClient => masaHttpClientBuilder.ConfigureHttpClient(httpClient));
            AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider
            =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
                return new HttpClientCaller(httpClient, serviceProvider, masaHttpClientBuilder.Prefix);
            });
            return httpClientBuilder;
        }
    }
}
