// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class CallerOptionsExtensions
{
    private static readonly string DefaultCallerName = Microsoft.Extensions.Options.Options.DefaultName;

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions)
        => callerOptions.UseHttpClient(DefaultCallerName);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        Action<MasaHttpClient> clientConfigure)
        => callerOptions.UseHttpClient(DefaultCallerName, clientConfigure);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions, string name)
        => callerOptions.UseHttpClientCore(name, null);

    public static MasaHttpClientBuilder UseHttpClient(this CallerOptions callerOptions,
        string name,
        Action<MasaHttpClient> clientConfigure)
    {
        MasaArgumentException.ThrowIfNull(name);
        MasaArgumentException.ThrowIfNull(clientConfigure);

        return callerOptions.UseHttpClientCore(name, clientConfigure);
    }

    private static MasaHttpClientBuilder UseHttpClientCore(this CallerOptions callerOptions,
        string name,
        Action<MasaHttpClient>? clientConfigure)
    {
        callerOptions.Services.AddHttpClient(name);
        AddCallerExtensions.AddCaller(callerOptions, name, serviceProvider =>
        {
            var masaHttpClient = new MasaHttpClient();
            clientConfigure?.Invoke(masaHttpClient);
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(name);
            masaHttpClient.ConfigureHttpClient(httpClient);
            return new HttpClientCaller(
                httpClient,
                serviceProvider,
                name,
                masaHttpClient.Prefix,
                masaHttpClient.RequestMessageFactory,
                masaHttpClient.ResponseMessageFactory);
        });
        return new MasaHttpClientBuilder(callerOptions.Services, name);
    }
}
