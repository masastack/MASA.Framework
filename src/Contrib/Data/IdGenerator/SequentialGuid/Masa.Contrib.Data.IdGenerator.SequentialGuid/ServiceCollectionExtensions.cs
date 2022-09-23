// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services)
        => services.AddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType)
        => services.AddSequentialGuidGenerator(Options.Options.DefaultName, guidType);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, string name)
        => services.AddSequentialGuidGenerator(name, SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection AddSequentialGuidGenerator(this IServiceCollection services, string name, SequentialGuidType guidType)
    {
        services.AddSequentialGuidGeneratorCore(guidType, name);
        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    public static IServiceCollection TestAddSequentialGuidGenerator(this IServiceCollection services)
        => services.TestAddSequentialGuidGenerator(SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection TestAddSequentialGuidGenerator(this IServiceCollection services, SequentialGuidType guidType)
        => services.TestAddSequentialGuidGenerator(Options.Options.DefaultName, guidType);

    public static IServiceCollection TestAddSequentialGuidGenerator(this IServiceCollection services, string name)
        => services.TestAddSequentialGuidGenerator(name, SequentialGuidType.SequentialAtEnd);

    public static IServiceCollection TestAddSequentialGuidGenerator(
        this IServiceCollection services,
        string name,
        SequentialGuidType guidType)
    {
        services.AddSequentialGuidGeneratorCore(guidType, name);
        MasaApp.Services = services;
        MasaApp.Build();
        return services;
    }

    private static IServiceCollection AddSequentialGuidGeneratorCore(
        this IServiceCollection services,
        SequentialGuidType guidType,
        string name)
    {
        if (services.Any(service => service.ImplementationType == typeof(SequentialGuidGeneratorProvider)))
            return services;

        services.AddSingleton<SequentialGuidGeneratorProvider>();

        ArgumentNullException.ThrowIfNull(name);

        services.AddIdGeneratorCore();
        services.TryAddSingleton<ISequentialGuidGenerator>(_ => new SequentialGuidGenerator(guidType));
        services.AddSingleton<IIdGenerator<Guid>>(serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());
        services.AddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>());

        services.Configure<IdGeneratorFactoryOptions>(factoryOptions =>
        {
            factoryOptions.Options.Add(new IdGeneratorRelationOptions(name)
            {
                Func = serviceProvider => serviceProvider.GetRequiredService<ISequentialGuidGenerator>()
            });
        });
        return services;
    }

    private sealed class SequentialGuidGeneratorProvider
    {

    }
}
