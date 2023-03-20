// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class MasaCallerClientBuilderExtensions
{
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder)
    {
        return UseAuthentication(masaCallerClientBuilder, (serviceProvider) => { return serviceProvider.GetRequiredService<TokenProvider>(); });
    }

    /// <summary>
    /// Caller adds default authentication
    /// </summary>
    /// <param name="masaCallerClientBuilder"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <returns></returns>
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        Func<IServiceProvider, TokenProvider> tokenProvider,
        string defaultScheme = AuthenticationConstant.DEFAULT_SCHEME)
    {
        masaCallerClientBuilder.Services.TryAddScoped((serviceProvider) => { return tokenProvider.Invoke(serviceProvider); });
        masaCallerClientBuilder.UseAuthentication(serviceProvider =>
            new AuthenticationService(
                serviceProvider.GetRequiredService<TokenProvider>(),
                defaultScheme
            ));
        return masaCallerClientBuilder;
    }
}
