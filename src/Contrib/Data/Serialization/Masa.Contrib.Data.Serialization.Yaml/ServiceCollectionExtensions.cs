// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYaml(this IServiceCollection services)
        => services.AddYaml(
            () => new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build(),
            () => new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build());

    public static IServiceCollection AddYaml(this IServiceCollection services,
        Action<SerializerBuilder> serializerAction,
        Action<DeserializerBuilder> deserializerAction)
    {
        return services.AddYaml(() =>
        {
            var serializerBuilder = new SerializerBuilder();
            serializerAction.Invoke(serializerBuilder);
            return serializerBuilder.Build();
        }, () =>
        {
            var deserializerBuilder = new DeserializerBuilder();
            deserializerAction.Invoke(deserializerBuilder);
            return deserializerBuilder.Build();
        });
    }

    public static IServiceCollection AddYaml(this IServiceCollection services,
        Func<YamlDotNet.Serialization.ISerializer> serializerFunc,
        Func<YamlDotNet.Serialization.IDeserializer> deserializerFunc)
    {
        if (services.Any(service => service.ImplementationType == typeof(YamlProvider)))
            return services;

        services.AddSingleton<YamlProvider>();

        services.AddSerializationCore();

        string name = DataType.Yml.ToString();
        services
            .AddYamlCore(serializerFunc.Invoke(), deserializerFunc.Invoke())
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

    private static IServiceCollection AddYamlCore(this IServiceCollection services,
        YamlDotNet.Serialization.ISerializer serializer,
        YamlDotNet.Serialization.IDeserializer deserializer)
    {
        services.TryAddSingleton<IYamlSerializer>(_ => new DefaultYamlSerializer(serializer));
        services.TryAddSingleton<IYamlDeserializer>(_ => new DefaultYamlDeserializer(deserializer));
        return services;
    }

    private sealed class YamlProvider
    {
    }
}
