// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSimpleGuidGenerator(this IServiceCollection services)
        => services.AddSimpleGuidGenerator(Options.Options.DefaultName);

    public static IServiceCollection AddSimpleGuidGenerator(this IServiceCollection services, string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));

        if (services.Any(service => service.ImplementationType == typeof(SimpleGuidGeneratorProvider)))
            return services;

        services.AddSingleton<SimpleGuidGeneratorProvider>();

        services.AddIdGeneratorCore();
        services.AddSingleton<IGuidGenerator, NormalGuidGenerator>();
        services.AddSingleton<IIdGenerator<Guid>, IGuidGenerator>();
        services.AddSingleton<IIdGenerator, IGuidGenerator>();

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
