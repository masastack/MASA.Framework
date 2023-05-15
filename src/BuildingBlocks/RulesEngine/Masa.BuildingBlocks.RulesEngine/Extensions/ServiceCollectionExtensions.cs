// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRulesEngine(this IServiceCollection services, Action<RulesEngineOptionsBuilder> action)
        => services.AddRulesEngine(Microsoft.Extensions.Options.Options.DefaultName, action);

    public static IServiceCollection AddRulesEngine(this IServiceCollection services, string name, Action<RulesEngineOptionsBuilder> action)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddTransient<IRulesEngineFactory, DefaultRulesEngineFactory>();

        services.TryAddSingleton<SingletonService<IRulesEngineClient>>(serviceProvider
            => new SingletonService<IRulesEngineClient>(serviceProvider.GetRequiredService<IRulesEngineFactory>().Create()));
        services.TryAddScoped<ScopedService<IRulesEngineClient>>(serviceProvider
            => new ScopedService<IRulesEngineClient>(serviceProvider.GetRequiredService<IRulesEngineFactory>().Create()));

        services.TryAddTransient(serviceProvider =>
        {
            if (serviceProvider.EnableIsolation())
                return serviceProvider.GetRequiredService<ScopedService<IRulesEngineClient>>().Service;

            return serviceProvider.GetRequiredService<SingletonService<IRulesEngineClient>>().Service;
        });
        var rulesEngineOptionsBuilder = new RulesEngineOptionsBuilder(services, name);
        action.Invoke(rulesEngineOptionsBuilder);
        return services;
    }
}
