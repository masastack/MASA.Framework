// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJson(this IServiceCollection services, Action<JsonSerializerOptions>? configure = null)
    {
        if (configure != null)
            services
                .Configure(Options.Options.DefaultName, configure)
                .AddJsonFactory(DataType.Json.ToString(), Options.Options.DefaultName);

        return services.AddJsonFactory(DataType.Json.ToString(), MasaApp.GetJsonSerializerOptions());
    }

    public static IServiceCollection AddJson(this IServiceCollection services, string name, Action<JsonSerializerOptions>? configure = null)
    {
        if (configure != null)
        {
            return services
                .Configure(name, configure)
                .AddJsonFactory(name, name);
        }
        return services.AddJsonFactory(name, MasaApp.GetJsonSerializerOptions());
    }

    private static IServiceCollection AddJsonFactory(
        this IServiceCollection services,
        string name,
        JsonSerializerOptions? jsonSerializerOptions)
    {
        return services
            .TryAddJsonCore(jsonSerializerOptions)
            .Configure<SerializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options
                    .MappingSerializer(name, _ => new DefaultJsonSerializer(jsonSerializerOptions));
            })
            .Configure<DeserializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options
                    .MappingDeserializer(name, _ => new DefaultJsonDeserializer(jsonSerializerOptions));
            });
    }

    private static IServiceCollection AddJsonFactory(this IServiceCollection services, string name, string optionName)
    {
        return services
            .TryAddJsonCore()
            .Configure<SerializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options.MappingSerializer(name, serviceProvider =>
                {
                    var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<JsonSerializerOptions>>();
                    var jsonSerializerOptions = optionsFactory.Create(optionName);
                    return new DefaultJsonSerializer(jsonSerializerOptions);
                });
            })
            .Configure<DeserializerFactoryOptions>(options =>
            {
                if (options.Options.Any(opt => opt.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                    return;

                options.MappingDeserializer(name, serviceProvider =>
                {
                    var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<JsonSerializerOptions>>();
                    var jsonSerializerOptions = optionsFactory.Create(optionName);
                    return new DefaultJsonDeserializer(jsonSerializerOptions);
                });
            });
    }

    private static IServiceCollection TryAddJsonCore(this IServiceCollection services, JsonSerializerOptions? jsonSerializerOptions = null)
    {
        services.TryAddSerializationCore();

        services.TryAddSingleton<IJsonSerializer>(serviceProvider =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<JsonSerializerOptions>>();
            return new DefaultJsonSerializer(jsonSerializerOptions ?? optionsFactory.Create(Options.Options.DefaultName));
        });
        services.TryAddSingleton<IJsonDeserializer>(serviceProvider =>
        {
            var optionsFactory = serviceProvider.GetRequiredService<IOptionsFactory<JsonSerializerOptions>>();
            return new DefaultJsonDeserializer(jsonSerializerOptions ?? optionsFactory.Create(Options.Options.DefaultName));
        });
        MasaApp.TrySetServiceCollection(services);
        return services;
    }
}
