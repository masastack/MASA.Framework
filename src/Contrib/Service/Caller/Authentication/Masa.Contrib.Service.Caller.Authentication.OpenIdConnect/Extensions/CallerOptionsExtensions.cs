// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class CallerOptionsExtensions
{
    private const string DEFAULT_SCHEME = "Bearer";

    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="callerOptions"></param>
    /// <param name="action">Extended Check Token</param>
    /// <returns></returns>
    public static CallerOptions UseAuthentication(
        this CallerOptions callerOptions,
        Action<AuthenticationOptions>? action)
    {
        callerOptions.UseAuthentication();
        AuthenticationOptions authenticationOptions = new AuthenticationOptions(callerOptions.Services);
        action?.Invoke(authenticationOptions);
        return callerOptions;
    }

    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="callerOptions"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <param name="action">Extended Check Token</param>
    /// <returns></returns>
    public static CallerOptions UseAuthentication(
        this CallerOptions callerOptions,
        string defaultScheme,
        Action<AuthenticationOptions>? action)
    {
        callerOptions.UseAuthentication(defaultScheme);
        AuthenticationOptions authenticationOptions = new AuthenticationOptions(callerOptions.Services);
        action?.Invoke(authenticationOptions);
        return callerOptions;
    }

    /// <summary>
    /// Caller uses Authentication
    /// </summary>
    /// <param name="callerOptions"></param>
    /// <param name="defaultScheme">The default scheme used as a fallback for all other schemes.</param>
    /// <returns></returns>
    public static CallerOptions UseAuthentication(
        this CallerOptions callerOptions,
        string defaultScheme = DEFAULT_SCHEME)
    {
        callerOptions.Services.AddScoped<TokenProvider>();
        callerOptions.Services.TryAddScoped<IAuthenticationService>(serviceProvider =>
            new AuthenticationService(serviceProvider.GetRequiredService<TokenProvider>(),
                serviceProvider.GetService<ITokenValidatorHandler>(),
                defaultScheme
            ));
        return callerOptions;
    }
}
