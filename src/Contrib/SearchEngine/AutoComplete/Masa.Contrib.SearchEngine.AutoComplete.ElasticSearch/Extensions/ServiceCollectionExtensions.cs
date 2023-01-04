// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    internal static void AddAutoCompleteCore(this IServiceCollection services,
        string name,
        Action<AutoCompleteRelationsOptions> action,
        ServiceLifetime serviceLifetime)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryOrUpdate(name, action);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create());

        var serviceDescriptor = new ServiceDescriptor(
            typeof(IAutoCompleteClient),
            serviceProvider => serviceProvider.GetRequiredService<IAutoCompleteFactory>().Create(),
            serviceLifetime);
        services.TryAdd(serviceDescriptor);
        MasaApp.TrySetServiceCollection(services);
    }

    private static void TryOrUpdate(this IServiceCollection services,
        string name,
        Action<AutoCompleteRelationsOptions> action)
    {
        services.Configure<AutoCompleteFactoryOptions>(factoryOptions =>
        {
            var options = new AutoCompleteRelationsOptions(name);
            action.Invoke(options);
            if (factoryOptions.Options.Any(relation => relation.Name == options.Name))
                throw new ArgumentException($"The caller name already exists, please change the name, the repeat name is [{options.Name}]");

            factoryOptions.Options.Add(options);
        });
    }
}
