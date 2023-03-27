// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class SerializationBuilderExtensions
{
    private static readonly YamlOptions DefaultYamlOptions = new()
    {
        Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build(),
        Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()
    };

    public static void UseYaml(this SerializationBuilder serializationBuilder, Action<YamlOptions>? configure = null)
    {
        var yamlOptions = new YamlOptions();
        configure?.Invoke(yamlOptions);

        var serializer = yamlOptions.Serializer ?? DefaultYamlOptions.Serializer!;
        var deserializer = yamlOptions.Deserializer ?? DefaultYamlOptions.Deserializer!;

        serializationBuilder.Services.TryAddYamlCore(serializer, deserializer);
        serializationBuilder.UseSerialization(
            _ => new DefaultYamlSerializer(serializer),
            _ => new DefaultYamlDeserializer(deserializer));
    }

    private static void TryAddYamlCore(
        this IServiceCollection services,
        YamlDotNet.Serialization.ISerializer serializer,
        YamlDotNet.Serialization.IDeserializer deserializer)
    {
        services.TryAddSingleton<IYamlSerializer>(_ => new DefaultYamlSerializer(serializer));

        services.TryAddSingleton<IYamlDeserializer>(_ => new DefaultYamlDeserializer(deserializer));
    }
}
