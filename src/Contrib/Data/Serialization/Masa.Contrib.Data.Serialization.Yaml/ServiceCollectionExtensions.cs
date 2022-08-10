// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYaml(this IServiceCollection services)
        => services.AddYaml(Options.Options.DefaultName);

    public static IServiceCollection AddYaml(this IServiceCollection services, string name)
    {
        if (services.Any(service => service.ImplementationType == typeof(YamlProvider)))
            return services;

        services.AddSingleton<YamlProvider>();

        services
            .AddYamlCore()
            .Configure<SerializerFactoryOptions>(options =>
            {
                options
                    .MappingSerializer(name,
                        serviceProvider => serviceProvider.GetRequiredService<IYamlSerializer>());
            })
            .Configure<DeserializerFactoryOptions>(options =>
            {
                options
                    .MappingDeserializer(name,
                        serviceProvider => serviceProvider.GetRequiredService<IYamlDeserializer>());
            });
        return services;
    }

    private static IServiceCollection AddYamlCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IYamlSerializer, DefaultYamlSerializer>();
        services.TryAddSingleton<IYamlDeserializer, DefaultYamlDeserializer>();
        return services;
    }

    private class YamlProvider
    {
    }
}
