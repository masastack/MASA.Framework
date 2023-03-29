// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Service.Caller;

public static class MasaCallerClientBuilderExtensions
{
    // ReSharper disable once InconsistentNaming
    public static IMasaCallerClientBuilder UseI18n(this IMasaCallerClientBuilder masaCallerClientBuilder)
        => masaCallerClientBuilder.AddMiddleware(_ => new CultureMiddleware());

    public static IMasaCallerClientBuilder AddMiddleware<TMiddleware>(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        ServiceLifetime middlewareLifetime = ServiceLifetime.Singleton)
        where TMiddleware : class, ICallerMiddleware
    {
        masaCallerClientBuilder.Services.TryAdd(ServiceDescriptor.Describe(typeof(TMiddleware), typeof(TMiddleware), middlewareLifetime));
        return masaCallerClientBuilder.AddMiddleware(serviceProvider => serviceProvider.GetRequiredService<TMiddleware>());
    }

    public static IMasaCallerClientBuilder AddMiddleware(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        Func<IServiceProvider, ICallerMiddleware> implementationFactory)
    {
        masaCallerClientBuilder.Services.Configure<CallerMiddlewareFactoryOptions>(middlewareOptions =>
        {
            middlewareOptions.AddMiddleware(masaCallerClientBuilder.Name, implementationFactory);
        });
        return masaCallerClientBuilder;
    }

    public static void UseAuthentication(
        this IMasaCallerClientBuilder masaCallerClientBuilder,
        Func<IServiceProvider, IAuthenticationService> implementationFactory)
    {
        MasaArgumentException.ThrowIfNull(masaCallerClientBuilder);

        AddAuthenticationCore(masaCallerClientBuilder.Services);

        masaCallerClientBuilder.Services.Configure<AuthenticationServiceFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation
                    => relation.Name.Equals(masaCallerClientBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The caller name already exists, please change the name, the repeat name is [{masaCallerClientBuilder.Name}]");

            factoryOptions.Options.Add(new AuthenticationServiceRelationOptions(masaCallerClientBuilder.Name, implementationFactory));
        });
    }

    private static void AddAuthenticationCore(IServiceCollection services)
    {
        services.TryAddTransient<IAuthenticationService>(serviceProvider
            => serviceProvider.GetRequiredService<IAuthenticationServiceFactory>().Create());
        services.TryAddTransient<IAuthenticationServiceFactory, DefaultAuthenticationServiceFactory>();
    }
}
