// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity")]
[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.BlazorServer")]
[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.BlazorWebAssembly")]
[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.BlazorServer.Tests")]
[assembly: InternalsVisibleTo("Masa.Contrib.Authentication.Identity.BlazorWebAssembly.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Authentication.Identity;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        Action<JsonSerializerOptions>? configure)
        => services.AddMasaIdentityCore(_ =>
        {

        }, configure);

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        Action<IdentityClaimOptions> configureOptions,
        Action<JsonSerializerOptions>? configure = null)
    {
        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();

        services.Configure(configureOptions);

        bool isInitialized = false;
        JsonSerializerOptions? jsonSerializerOptions = null;
        return services.AddMasaIdentityModelCore(_ =>
        {
            JsonSerializerOptions? jsonSerializerOptionsTemp = jsonSerializerOptions;
            if (!isInitialized)
            {
                if (configure != null)
                {
                    jsonSerializerOptionsTemp = new JsonSerializerOptions();
                    configure.Invoke(jsonSerializerOptionsTemp);
                }
                jsonSerializerOptions = jsonSerializerOptionsTemp;
                isInitialized = true;
            }
            var deserializer = new DefaultJsonDeserializer(jsonSerializerOptionsTemp);
            var typeConvertProvider = new DefaultTypeConvertProvider(deserializer);
            return typeConvertProvider;
        });
    }

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        string serializationName)
        => services.AddMasaIdentityCore(serializationName, _ =>
        {
        });

    public static IServiceCollection AddMasaIdentityCore(
        this IServiceCollection services,
        string serializationName,
        Action<IdentityClaimOptions> configureOptions)
    {
        MasaArgumentException.ThrowIfNull(serializationName);
        MasaArgumentException.ThrowIfNull(configureOptions);

        if (services.Any<IdentityProvider>())
            return services;

        services.AddSingleton<IdentityProvider>();

        services.Configure(configureOptions);

        return services.AddMasaIdentityModelCore(serviceProvider =>
        {
            if (serviceProvider.GetService<IDeserializerFactory>()?.TryCreate(serializationName, out var deserializer) ?? false)
                return new DefaultTypeConvertProvider(deserializer);

            return new DefaultTypeConvertProvider(new DefaultJsonDeserializer(new JsonSerializerOptions()));
        });
    }

    private static IServiceCollection AddMasaIdentityModelCore(
        this IServiceCollection services,
        Func<IServiceProvider, ITypeConvertProvider> func)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddScoped<DefaultUserContext>(serviceProvider => new DefaultUserContext(
            func.Invoke(serviceProvider),
            serviceProvider.GetRequiredService<ICurrentPrincipalAccessor>(),
            serviceProvider.GetRequiredService<IOptionsMonitor<IdentityClaimOptions>>())
        );
        services.TryAddScoped<IUserSetter>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IUserContext>(serviceProvider => serviceProvider.GetService<DefaultUserContext>()!);
        services.TryAddScoped<IMultiTenantUserContext>(serviceProvider => new DefaultMultiTenantUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)
        ));
        services.TryAddScoped<IMultiEnvironmentUserContext>(serviceProvider => new DefaultMultiEnvironmentUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)));
        services.TryAddScoped<IIsolatedUserContext>(serviceProvider => new DefaultIsolatedUserContext(
            serviceProvider.GetRequiredService<IUserContext>(),
            func.Invoke(serviceProvider)));
        return services;
    }

#pragma warning disable S2094
    private sealed class IdentityProvider
    {
    }
#pragma warning restore S2094
}
