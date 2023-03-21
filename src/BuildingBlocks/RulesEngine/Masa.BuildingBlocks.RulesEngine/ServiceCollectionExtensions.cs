﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRulesEngine(this IServiceCollection services, Action<RulesEngineOptions> action)
        => services.AddRulesEngine(Microsoft.Extensions.Options.Options.DefaultName, action);

    public static IServiceCollection AddRulesEngine(this IServiceCollection services, string name, Action<RulesEngineOptions> action)
    {
        MasaApp.TrySetServiceCollection(services);
        services.TryAddSingleton<IRulesEngineFactory, DefaultRulesEngineFactory>();
        services.TryAddTransient(serviceProvider => serviceProvider.GetRequiredService<IRulesEngineFactory>().Create());
        RulesEngineOptions rulesEngineOptions = new RulesEngineOptions(services, name);
        action.Invoke(rulesEngineOptions);
        services.AddServiceFactory();
        return services;
    }
}