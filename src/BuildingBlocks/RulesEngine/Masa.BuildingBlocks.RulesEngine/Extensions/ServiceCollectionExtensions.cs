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
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IRulesEngineFactory>().Create());
        var rulesEngineOptionsBuilder = new RulesEngineOptionsBuilder(services, name);
        action.Invoke(rulesEngineOptionsBuilder);
        services.AddServiceFactory();
        return services;
    }
}
