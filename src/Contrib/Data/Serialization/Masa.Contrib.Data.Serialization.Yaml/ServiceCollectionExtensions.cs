// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYaml(this IServiceCollection services)
    {
        if (services.Any(service => service.ImplementationType == typeof(YamlProvider)))
            return services;

        services.AddSingleton<YamlProvider>();

        services.TryAddSingleton<IYamlSerializer, DefaultYamlSerializer>();
        services.TryAddSingleton<IYamlDeserializer, DefaultYamlDeserializer>();
        services.Configure<SerializerOptions>(serializerOptions =>
        {
            serializerOptions
                .MappingSerializer(DataType.Yml.ToString(),
                    serviceProvider => serviceProvider.GetRequiredService<IYamlSerializer>())
                .MappingDeserializer(DataType.Yml.ToString(),
                    serviceProvider => serviceProvider.GetRequiredService<IYamlDeserializer>());
        });
        return services;
    }

    private class YamlProvider
    {
    }
}
