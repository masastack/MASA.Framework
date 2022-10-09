// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
using Masa.Contrib.StackSdks.Sso;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaOpenIdConnect(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services.AddMasaOpenIdConnect(configuration.GetSection("$public.OIDC").Get<MasaOpenIdConnectOptions>());
    }

    public static IServiceCollection AddMasaOpenIdConnect(
        this IServiceCollection services,
        MasaOpenIdConnectOptions masaOpenIdConnectOptions)
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
            options.Authority = masaOpenIdConnectOptions.Authority;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.RequireHttpsMetadata = false;
            options.ClientId = masaOpenIdConnectOptions.ClientId;
            options.ClientSecret = masaOpenIdConnectOptions.ClientSecret;
            options.ResponseType = OpenIdConnectResponseType.Code;
            foreach (var scope in masaOpenIdConnectOptions.Scopes.ToArray())
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
                },
                OnRedirectToIdentityProviderForSignOut = context =>
                {
                    if (context.Properties.Items.ContainsKey("env"))
                    {
                        context.ProtocolMessage.SetParameter("env",
                            context.Properties.Items["env"]);
                    }
                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProvider = context =>
                {
                    return Task.CompletedTask;
                },
                OnSignedOutCallbackRedirect = context =>
                {
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy
            options.FallbackPolicy = options.DefaultPolicy;
        });

        services.AddSingleton(masaOpenIdConnectOptions);
        services.AddJwtTokenValidator(options =>
        {
            options.AuthorityEndpoint = masaOpenIdConnectOptions.Authority;
        });

        services.AddScoped<IIdentityProvider, IdentityProvider>();
        return services;
    }

    static IServiceCollection AddJwtTokenValidator(this IServiceCollection services,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptions)
    {
        var options = new JwtTokenValidatorOptions();
        jwtTokenValidatorOptions.Invoke(options);
        services.AddSingleton(options);
        services.AddScoped<IJwtTokenValidator, JwtTokenValidator>();
        return services;
    }
}
