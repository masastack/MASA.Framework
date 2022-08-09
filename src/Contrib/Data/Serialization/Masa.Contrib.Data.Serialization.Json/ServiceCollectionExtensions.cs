// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJson(this IServiceCollection services)
        => services.AddJson(Options.Options.DefaultName);

    public static IServiceCollection AddJson(this IServiceCollection services, string name)
    {
        if (services.Any(service => service.ImplementationType == typeof(JsonProvider)))
            return services;

        services.AddSingleton<JsonProvider>();

        services.TryAddSingleton<IJsonSerializer, DefaultJsonSerializer>();
        services.TryAddSingleton<IJsonDeserializer, DefaultJsonDeserializer>();
        services.Configure<SerializerOptions>(serializerOptions =>
        {
            serializerOptions
                .MappingSerializer(name,
                    serviceProvider => serviceProvider.GetRequiredService<IJsonSerializer>())
                .MappingDeserializer(name,
                    serviceProvider => serviceProvider.GetRequiredService<IJsonDeserializer>());
        });
        return services;
    }

    private class JsonProvider
    {
    }
}
