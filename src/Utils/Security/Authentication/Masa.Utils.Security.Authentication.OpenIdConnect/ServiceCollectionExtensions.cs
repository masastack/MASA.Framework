// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.Utils.Security.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaOpenIdConnect(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddMasaOpenIdConnect(configuration.GetSection("oidc").Get<MasaOpenIdConnectOptions>());
    }

    public static IServiceCollection AddMasaOpenIdConnect(
        this IServiceCollection services,
        MasaOpenIdConnectOptions masaOpenIdConnectOptions)
    {
        return services.AddMasaOpenIdConnect(masaOpenIdConnectOptions.Authority, masaOpenIdConnectOptions.ClientId,
                masaOpenIdConnectOptions.ClientSecret, masaOpenIdConnectOptions.Scopes.ToArray());
    }

    public static IServiceCollection AddMasaOpenIdConnect(
        this IServiceCollection services,
        string authority,
        string clinetId,
        string clientSecret,
        params string[] scopes)
    {
        services.AddHttpContextAccessor();

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromSeconds(5);
        })
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.RequireHttpsMetadata = false;
                options.ClientId = clinetId;
                options.ClientSecret = clientSecret;
                options.ResponseType = OpenIdConnectResponseType.Code;
                foreach (var scope in scopes)
                {
                    options.Scope.Add(scope);
                }

                options.SaveTokens = true;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.UseTokenLifetime = true;

                options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(5.0);
                options.TokenValidationParameters.RequireExpirationTime = true;
                options.TokenValidationParameters.ValidateLifetime = true;

                options.NonceCookie.SameSite = SameSiteMode.Unspecified;
                options.CorrelationCookie.SameSite = SameSiteMode.Unspecified;

                options.ClaimActions.MapUniqueJsonKey("account", "account");
                options.ClaimActions.MapUniqueJsonKey("roles", "roles");
                options.ClaimActions.MapUniqueJsonKey("environment", "environment");
                options.ClaimActions.MapUniqueJsonKey("current_team", "current_team");
                options.ClaimActions.MapUniqueJsonKey("staff_id", "staff_id");

                options.Events = new OpenIdConnectEvents
                {
                    OnAccessDenied = context =>
                    {
                        context.HandleResponse();
                        context.Response.Redirect("/");
                        return Task.CompletedTask;
                    },
                    OnRemoteFailure = context =>
                    {
                        if (context.HttpContext.Request.Path.Value == "/signin-oidc")
                        {
                            context.SkipHandler();
                            context.Response.Redirect("/");
                        }
                        else
                        {
                            context.HandleResponse();
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy
            options.FallbackPolicy = options.DefaultPolicy;
        });

        return services;
    }
}

