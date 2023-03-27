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

        return services.AddAuthClient(callerOptionsBuilder =>
        {
            callerOptionsBuilder
                .UseHttpClient(builder => builder.BaseAddress = authServiceBaseAddress)
                .UseAuthentication();
        }, redisOptions);
    }

    private static IServiceCollection AddAuthClient(this IServiceCollection services, Action<CallerOptionsBuilder> callerOptionsBuilder,
        RedisConfigurationOptions redisOptions)
    {
        MasaArgumentException.ThrowIfNull(callerOptionsBuilder);
        if (services.All(service => service.ServiceType != typeof(IMultiEnvironmentUserContext)))
            throw new Exception("Please add IMultiEnvironmentUserContext first.");

        services.TryAddScoped<IEnvironmentProvider, EnvironmentProvider>();
        services.AddCaller(DEFAULT_CLIENT_NAME, callerOptionsBuilder);

        services.AddAuthClientMultilevelCache(redisOptions);
        services.AddScoped<IAuthClient>(serviceProvider =>
        {
            var userContext = serviceProvider.GetRequiredService<IMultiEnvironmentUserContext>();
            var caller = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var authClientMultilevelCacheProvider = serviceProvider.GetRequiredService<AuthClientMultilevelCacheProvider>();
            var authClient = new AuthClient(caller, userContext, authClientMultilevelCacheProvider.GetMultilevelCacheClient());
            return authClient;
        });
        services.AddScoped<IThirdPartyIdpService>(serviceProvider =>
        {
            var callProvider = serviceProvider.GetRequiredService<ICallerFactory>().Create(DEFAULT_CLIENT_NAME);
            var authClientMultilevelCacheProvider = serviceProvider.GetRequiredService<AuthClientMultilevelCacheProvider>();
            var thirdPartyIdpService = new ThirdPartyIdpService(callProvider, authClientMultilevelCacheProvider.GetMultilevelCacheClient());
            return thirdPartyIdpService;
        });

        MasaApp.TrySetServiceCollection(services);

        return services;
    }

    public static IServiceCollection AddSsoClient(this IServiceCollection services, IConfiguration configuration)
    {
        var ssoServiceBaseAddressFunc = () => configuration.GetValue<string>("$public.AppSettings:SsoClient:Url");
        services.AddSsoClient(ssoServiceBaseAddressFunc);

        return services;
    }

    public static IServiceCollection AddSsoClient(this IServiceCollection services, Func<string> ssoServiceBaseAddressFunc)
    {
        services.AddHttpClient(DEFAULT_SSO_CLIENT_NAME, httpClient =>
        {
            httpClient.BaseAddress = new Uri(ssoServiceBaseAddressFunc());
        });
        services.AddSingleton<ISsoClient, SsoClient>();

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
