// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthClient(this IServiceCollection services, IConfiguration configuration,
        string? authServiceBaseAddress = null)
    {
        var redisOptions = configuration.GetSection("$public.RedisConfig").Get<RedisConfigurationOptions>();
        authServiceBaseAddress ??= configuration.GetValue<string>("$public.AppSettings:AuthClient:Url");
        services.AddAuthClient(authServiceBaseAddress, redisOptions);

        return services;
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, string authServiceBaseAddress,
        RedisConfigurationOptions redisOptions)
    {
        MasaArgumentException.ThrowIfNullOrEmpty(authServiceBaseAddress);

        return services.AddAuthClient(callerOptions =>
        {
            callerOptions.UseHttpClient(DEFAULT_CLIENT_NAME, builder =>
            {
                builder.BaseAddress = authServiceBaseAddress;
            }).AddMiddleware(_ => new AuthenticationMiddleware());
            callerOptions.DisableAutoRegistration = true;
        }, redisOptions);
    }

    public static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptions> callerOptions,
        RedisConfigurationOptions redisOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptions);
        if (services.All(service => service.ServiceType != typeof(IMultiEnvironmentUserContext)))
            throw new Exception("Please add IMultiEnvironmentUserContext first.");

        services.AddHttpContextAccessor();
        services.TryAddScoped<IEnvironmentProvider, EnvironmentProvider>();
        services.AddCaller(callerOptions);

        services.AddAuthClientMultilevelCache(redisOptions);
        services.AddSingleton<IThirdPartyIdpCacheService, ThirdPartyIdpCacheService>();
        services.AddSingleton<ISsoClient, SsoClient>();
        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var userContext = serviceProvider.GetRequiredService<IMultiEnvironmentUserContext>();
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var authClientMultilevelCacheProvider = serviceProvider.GetRequiredService<AuthClientMultilevelCacheProvider>();
            var authClient = new AuthClient(callProvider, userContext, authClientMultilevelCacheProvider.GetMultilevelCacheClient());
            return authClient;
        });

        MasaApp.TrySetServiceCollection(services);

        return services;
    }

    public static IServiceCollection AddAuthClientMultilevelCache(this IServiceCollection services, RedisConfigurationOptions redisOptions)
    {
        services.AddMultilevelCache(
            DEFAULT_CLIENT_NAME,
            distributedCacheOptions => distributedCacheOptions.UseStackExchangeRedisCache(redisOptions),
            multilevelCacheOptions =>
            {
                multilevelCacheOptions.SubscribeKeyType = SubscribeKeyType.SpecificPrefix;
                multilevelCacheOptions.SubscribeKeyPrefix = $"{DEFAULT_SUBSCRIBE_KEY_PREFIX}-db-{redisOptions.DefaultDatabase}";
            }
        );
        services.AddSingleton<AuthClientMultilevelCacheProvider>();

        return services;
    }
}
