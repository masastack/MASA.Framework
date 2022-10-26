// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRulesEngine(this IServiceCollection services, Action<RulesEngineOptions> action)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IRulesEngineFactory, DefaultRulesEngineFactory>();
        services.TryAddTransient(serviceProvider => serviceProvider.GetRequiredService<IRulesEngineFactory>().Create());
        RulesEngineOptions rulesEngineOptions = new RulesEngineOptions(services);
        action.Invoke(rulesEngineOptions);
        return services;
    }
}
