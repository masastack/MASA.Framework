// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthClient(this IServiceCollection services, IConfiguration configuration, string? authServiceBaseAddress = null)
    {
        var redisOptions = configuration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
        authServiceBaseAddress ??= configuration.GetValue<string>("$public.AppSettings:AuthClient:Url");
        services.AddAuthClient(authServiceBaseAddress, redisOptions);

        return services;
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, string authServiceBaseAddress, RedisConfigurationOptions redisOptions)
    {
        ArgumentNullException.ThrowIfNull(authServiceBaseAddress, nameof(authServiceBaseAddress));

        return services.AddAuthClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.Configure = opt => opt.BaseAddress = new Uri(authServiceBaseAddress);
            })
            .AddHttpMessageHandler<HttpEnvironmentDelegatingHandler>();
            callerOptions.DisableAutoRegistration = true;
        }, redisOptions);
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptions> callerOptions, RedisConfigurationOptions redisOptions)
    {
        ArgumentNullException.ThrowIfNull(callerOptions, nameof(callerOptions));
        if (services.All(service => service.ServiceType != typeof(IMultiEnvironmentUserContext)))
            throw new Exception("Please add IMultiEnvironmentUserContext first.");

        services.AddHttpContextAccessor();
        services.TryAddScoped<IEnvironmentProvider, EnvironmentProvider>();
        services.AddScoped<HttpEnvironmentDelegatingHandler>();
        services.AddCaller(callerOptions);
        services.AddStackExchangeRedisCache(DEFAULT_CLIENT_NAME, redisOptions).AddMultilevelCache(options =>
        {
            options.SubscribeKeyType = SubscribeKeyType.SpecificPrefix;
            options.SubscribeKeyPrefix = DEFAULT_SUBSCRIBE_KEY_PREFIX;
        });

        services.AddSingleton<IThirdPartyIdpCacheService, ThirdPartyIdpCacheService>();
        services.AddSingleton<ISsoClient, SsoClient>();
        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var tokenProvider = serviceProvider.GetService<TokenProvider>();
            var userContext = serviceProvider.GetRequiredService<IMultiEnvironmentUserContext>();
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            if (tokenProvider != null)
            {
                callProvider.ConfigRequestMessage(httpRequestMessage =>
                {
                    httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenProvider.AccessToken);
                });
            }
            var authClient = new AuthClient(callProvider, userContext);
            return authClient;
        });

        MasaApp.TrySetServiceCollection(services);

        return services;
    }
}
