// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services)
        => services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType)
        => services.AddSequentialGuidGenerator(guidType, Options.Options.DefaultName);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, string name)
        => services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd, name);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType, string name)
    {
        if (services.Any(service => service.ImplementationType == typeof(SequentialGuidGeneratorProvider)))
            return services;

        services.AddSingleton<SequentialGuidGeneratorProvider>();

        services.AddIdGeneratorCore();
        services.TryAddSingleton<ISequentialGuidGenerator>(_ => new SequentialGuidGenerator(guidType));
        services.AddSingleton<IIdGenerator<Guid>>(serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());
        services.AddSingleton<IIdGenerator, ISequentialGuidGenerator>();

        services.Configure<IdGeneratorFactoryOptions>(factoryOptions =>
        {
            factoryOptions.Options.Add(new IdGeneratorRelationOptions(name)
            {
                Func = serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>()
            });
        });

        return services;
    }

    private class SequentialGuidGeneratorProvider
    {

    }
}
