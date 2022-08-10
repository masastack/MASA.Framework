// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJson(this IServiceCollection services)
    {
        if (services.Any(service => service.ImplementationType == typeof(JsonProvider)))
            return services;

        services.AddSingleton<JsonProvider>();

        services.AddSerializationCore();
        services.TryAddSingleton<IJsonSerializer, DefaultJsonSerializer>();
        services.TryAddSingleton<IJsonDeserializer, DefaultJsonDeserializer>();
        string name = DataType.Json.ToString();
        services.Configure<SerializerFactoryOptions>(options =>
        {
            options
                .MappingSerializer(name,
                    serviceProvider => serviceProvider.GetRequiredService<IJsonSerializer>());
        });
        services.Configure<DeserializerFactoryOptions>(options =>
        {
            options
                .MappingDeserializer(name,
                    serviceProvider => serviceProvider.GetRequiredService<IJsonDeserializer>());
        });
        return services;
    }

    private sealed class JsonProvider
    {
    }
}
