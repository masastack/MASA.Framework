// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    internal static void AddAutoCompleteCore(this IServiceCollection services,
        string name,
        Action<AutoCompleteRelationsOptions> action,
        ServiceLifetime serviceLifetime)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryOrUpdate(name, action);

        services.TryAddSingleton<IAutoCompleteFactory, AutoCompleteFactory>();

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
            var relationsOptions = new AutoCompleteRelationsOptions(name);
            action.Invoke(relationsOptions);
            if (factoryOptions.Options.Any(relation => relation.Name == relationsOptions.Name))
                throw new ArgumentException($"The caller name already exists, please change the name, the repeat name is [{relationsOptions.Name}]");

            factoryOptions.Options.Add(relationsOptions);
        });
    }
}
