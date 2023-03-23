// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class MasaCallerClientBuilderExtensions
{
    /// <summary>
    /// Caller adds default authentication
    /// </summary>
    /// <param name="masaCallerClientBuilder"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <returns></returns>
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder)
    {
        masaCallerClientBuilder.Services.TryAddScoped<ITokenGenerater, DefaultTokenGenerater>();
        masaCallerClientBuilder.Services.TryAddScoped(s => s.GetRequiredService<ITokenGenerater>().Generater());
        masaCallerClientBuilder.UseAuthentication(serviceProvider =>
            new AuthenticationService(
                serviceProvider.GetRequiredService<TokenProvider>(),
                null
            ));
        return masaCallerClientBuilder;
    }
}
