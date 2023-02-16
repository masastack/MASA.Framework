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
        this IMasaCallerClientBuilder masaCallerClientBuilder)
        where TMiddleware : class, ICallerMiddleware
    {
        masaCallerClientBuilder.Services.TryAddSingleton<TMiddleware>();
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
}
