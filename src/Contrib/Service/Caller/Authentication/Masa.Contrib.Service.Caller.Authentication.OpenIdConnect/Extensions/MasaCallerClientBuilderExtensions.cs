// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class MasaCallerClientBuilderExtensions
{
    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="masaCallerClientBuilder"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <returns></returns>
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        string defaultScheme = Constant.DEFAULT_SCHEME)
    {
        masaCallerClientBuilder.Services.AddHttpContextAccessor();
        masaCallerClientBuilder.Services.AddScoped<TokenProvider>();
        masaCallerClientBuilder.AddMiddleware(serviceProvider =>
            new AuthenticationMiddleware(
                serviceProvider.GetRequiredService<IHttpContextAccessor>(),
                defaultScheme
            ));
        return masaCallerClientBuilder;
    }

    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="masaCallerClientBuilder"></param>
    /// <param name="action">Extended Check Token</param>
    /// <returns></returns>
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        Action<AuthenticationOptions> action)
        => masaCallerClientBuilder.UseAuthentication(Constant.DEFAULT_SCHEME, action);

    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="masaCallerClientBuilder"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <param name="action">Extended Check Token</param>
    /// <returns></returns>
    public static IMasaCallerClientBuilder UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        string defaultScheme,
        Action<AuthenticationOptions> action)
    {
        MasaArgumentException.ThrowIfNull(action);

        masaCallerClientBuilder.UseAuthentication(defaultScheme);
        var authenticationOptions = new AuthenticationOptions(masaCallerClientBuilder.Services);
        action(authenticationOptions);
        return masaCallerClientBuilder;
    }
}
