// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static YamlOptions DefaultYamlOptions = new()
    {
        Serializer = new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build(),
        Deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build()
    };

    public static IServiceCollection AddYaml(this IServiceCollection services, Action<YamlOptions>? configure = null)
    {
        if (configure != null)
        {
            return services
                .Configure(Options.Options.DefaultName, configure)
                .AddYamlFactory(DataType.Yml.ToString(), Options.Options.DefaultName);
        }

        return services.AddYamlFactory(DataType.Yml.ToString(), DefaultYamlOptions.Serializer, DefaultYamlOptions.Deserializer);
    }

    public static IServiceCollection AddYaml(this IServiceCollection services, string name, Action<YamlOptions>? configure = null)
    {
        if (configure != null)
        {
            return services
                .Configure(name, configure)
                .AddYamlFactory(name, name);
        }

        return services.AddYamlFactory(name, DefaultYamlOptions.Serializer, DefaultYamlOptions.Deserializer);
    }

    private static IServiceCollection AddYamlFactory(this IServiceCollection services, string name, string optionName)
    {
        return services
            .TryAddYamlCore()
            .Configure<SerializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options.MappingSerializer(name, serviceProvider =>
                {
                    var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<YamlOptions>>();
                    return new DefaultYamlSerializer(optionsFactory.Create(optionName).Serializer);
                });
            })
            .Configure<DeserializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options.MappingDeserializer(name, serviceProvider =>
                {
                    var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<YamlOptions>>();
                    return new DefaultYamlDeserializer(optionsFactory.Create(optionName).Deserializer);
                });
            });
    }

    private static IServiceCollection AddYamlFactory(this IServiceCollection services,
        string name,
        YamlDotNet.Serialization.ISerializer serializer,
        YamlDotNet.Serialization.IDeserializer deserializer)
    {
        return services
            .TryAddYamlCore(serializer, deserializer)
            .Configure<SerializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options
                    .MappingSerializer(name, _ => new DefaultYamlSerializer(serializer));
            })
            .Configure<DeserializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options
                    .MappingDeserializer(name, _ => new DefaultYamlDeserializer(deserializer));
            });
    }

    private static IServiceCollection TryAddYamlCore(this IServiceCollection services,
        YamlDotNet.Serialization.ISerializer? serializer = null,
        YamlDotNet.Serialization.IDeserializer? deserializer = null)
    {
        services.TryAddSerializationCore();
        services.TryAddSingleton<IYamlSerializer>(serviceProvider =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<YamlOptions>>();
            return new DefaultYamlSerializer(serializer ?? optionsFactory.Create(Options.Options.DefaultName).Serializer);
        });
        services.TryAddSingleton<IYamlDeserializer>(serviceProvider =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<YamlOptions>>();
            return new DefaultYamlDeserializer(deserializer ?? optionsFactory.Create(Options.Options.DefaultName).Deserializer);
        });
        return services;
    }
}
