// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleGuidGenerator(this IServiceCollection services)
        => services.AddSimpleGuidGenerator(Options.Options.DefaultName);

    public static IServiceCollection AddSimpleGuidGenerator(this IServiceCollection services, string name)
    {
        services.AddSimpleGuidGeneratorCore(name);
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static IServiceCollection TestAddSimpleGuidGenerator(this IServiceCollection services)
        => services.TestAddSimpleGuidGenerator(Options.Options.DefaultName);

    public static IServiceCollection TestAddSimpleGuidGenerator(this IServiceCollection services, string name)
    {
        services.AddSimpleGuidGeneratorCore(name);
        MasaApp.SetServiceCollection(services);
        MasaApp.Build();
        return services;
    }

    private static IServiceCollection AddSimpleGuidGeneratorCore(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (services.Any(service => service.ImplementationType == typeof(SimpleGuidGeneratorProvider)))
            return services;

        services.AddSingleton<SimpleGuidGeneratorProvider>();

        services.AddIdGeneratorCore();
        services.AddSingleton<IGuidGenerator, NormalGuidGenerator>();
        services.AddSingleton<IIdGenerator<Guid>>(serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>());
        services.AddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>());

        services.Configure<IdGeneratorFactoryOptions>(factoryOptions =>
        {
            factoryOptions.Options.Add(new IdGeneratorRelationOptions(name)
            {
                Func = serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>()
            });
        });
        return services;
    }

    private sealed class SimpleGuidGeneratorProvider
    {
    }
}
