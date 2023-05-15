// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerBuilderExtensions
{
    public static MasaHttpClientBuilder UseHttpClient(this CallerBuilder callerBuilder)
        => callerBuilder.UseHttpClientCore(null);

    public static MasaHttpClientBuilder UseHttpClient(this CallerBuilder callerBuilder,
        Action<MasaHttpClient> clientConfigure)
    {
        MasaArgumentException.ThrowIfNull(clientConfigure);

        return callerBuilder.UseHttpClientCore(clientConfigure);
    }

    private static MasaHttpClientBuilder UseHttpClientCore(this CallerBuilder callerBuilder,
        Action<MasaHttpClient>? clientConfigure)
    {
        callerBuilder.Services.AddHttpClient(callerBuilder.Name);

        callerBuilder.UseCustomCaller(serviceProvider =>
        {
            var masaHttpClient = new MasaHttpClient(serviceProvider);
            clientConfigure?.Invoke(masaHttpClient);
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(callerBuilder.Name);
            masaHttpClient.ConfigureHttpClient(httpClient);
            return new HttpClientCaller(
                httpClient,
                serviceProvider,
                callerBuilder.Name,
                masaHttpClient.Prefix,
                masaHttpClient.RequestMessageFactory,
                masaHttpClient.ResponseMessageFactory);
        });
        return new MasaHttpClientBuilder(callerBuilder.Services, callerBuilder.Name);
    }
}
