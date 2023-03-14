// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    public static MasaHttpClientBuilder UseHttpClient(this CallerOptionsBuilder callerOptionsBuilder)
        => callerOptionsBuilder.UseHttpClientCore(null);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptionsBuilder callerOptionsBuilder,
        Action<MasaHttpClient> clientConfigure)
    {
        MasaArgumentException.ThrowIfNull(clientConfigure);

        return callerOptionsBuilder.UseHttpClientCore(clientConfigure);
    }

    private static MasaHttpClientBuilder UseHttpClientCore(this CallerOptionsBuilder callerOptionsBuilder,
        Action<MasaHttpClient>? clientConfigure)
    {
        callerOptionsBuilder.Services.AddHttpClient(callerOptionsBuilder.Name);

        callerOptionsBuilder.AddCallerRelation(serviceProvider =>
        {
            var masaHttpClient = new MasaHttpClient(serviceProvider);
            clientConfigure?.Invoke(masaHttpClient);
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(callerOptionsBuilder.Name);
            masaHttpClient.ConfigureHttpClient(httpClient);
            return new HttpClientCaller(
                httpClient,
                serviceProvider,
                callerOptionsBuilder.Name,
                callerOptionsBuilder.Lifetime != ServiceLifetime.Singleton,
                masaHttpClient.Prefix,
                masaHttpClient.RequestMessageFactory,
                masaHttpClient.ResponseMessageFactory);
        });
        return new MasaHttpClientBuilder(callerOptionsBuilder.Services, callerOptionsBuilder.Name);
    }
}
