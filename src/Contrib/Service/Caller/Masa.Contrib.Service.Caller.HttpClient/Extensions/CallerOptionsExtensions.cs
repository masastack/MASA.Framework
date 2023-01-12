// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    private static readonly string DefaultCallerName = Microsoft.Extensions.Options.Options.DefaultName;

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        Func<MasaHttpClient>? configureClient = null,
        bool alwaysGetNewestHttpClient = false)
        => callerOptions.UseHttpClient(DefaultCallerName, configureClient, alwaysGetNewestHttpClient);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        string name,
        Func<MasaHttpClient>? configureClient = null,
        bool alwaysGetNewestHttpClient = false)
    {
        return callerOptions.UseHttpClient(name, masaHttpClientBuilder =>
        {
            var builder = configureClient == null ? new MasaHttpClient() : configureClient.Invoke();
            masaHttpClientBuilder.BaseAddress = builder.BaseAddress;
            masaHttpClientBuilder.Prefix = builder.Prefix;
            masaHttpClientBuilder.Configure = builder.Configure;
        }, alwaysGetNewestHttpClient);
    }

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        Action<MasaHttpClient>? configureClient,
        bool alwaysGetNewestHttpClient = false)
        => callerOptions.UseHttpClient(DefaultCallerName, configureClient, alwaysGetNewestHttpClient);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        string name,
        Action<MasaHttpClient>? configureClient,
        bool alwaysGetNewestHttpClient = false)
    {
        if (alwaysGetNewestHttpClient)
        {
            callerOptions.Services.AddHttpClient(name);
            AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider =>
            {
                var masaHttpClientBuilder = new MasaHttpClient();
                configureClient?.Invoke(masaHttpClientBuilder);
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
                masaHttpClientBuilder.ConfigureHttpClient(httpClient);
                return new HttpClientCaller(
                    httpClient,
                    serviceProvider,
                    name,
                    masaHttpClientBuilder.Prefix,
                    masaHttpClientBuilder.RequestMessageFactory,
                    masaHttpClientBuilder.ResponseMessageFactory);
            });
        }
        else
        {
            var masaHttpClient = new MasaHttpClient();
            configureClient?.Invoke(masaHttpClient);
            callerOptions.Services.AddHttpClient(name, httpClient => masaHttpClient.ConfigureHttpClient(httpClient));
            AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider =>
            {
                var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
                return new HttpClientCaller(
                    httpClient,
                    serviceProvider,
                    name,
                    masaHttpClient.Prefix,
                    masaHttpClient.RequestMessageFactory,
                    masaHttpClient.ResponseMessageFactory);
            });
        }
        return new MasaHttpClientBuilder(callerOptions.Services, name);
    }
}
