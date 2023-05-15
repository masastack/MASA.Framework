// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSerialization(
        this IServiceCollection services,
        Action<SerializationBuilder> configure)
        => services.AddSerialization(Options.Options.DefaultName, configure);

    public static IServiceCollection AddSerialization(
        this IServiceCollection services,
        string name,
        Action<SerializationBuilder> configure)
    {
        MasaApp.TrySetServiceCollection(services);
        var builder = new SerializationBuilder(name, services);
        configure.Invoke(builder);
        return services.AddSerializationCore();
    }

    private static IServiceCollection AddSerializationCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IDeserializerFactory, DefaultDeserializerFactory>();
        services.TryAddSingleton<ISerializerFactory, DefaultSerializerFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ISerializerFactory>().Create());
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IDeserializerFactory>().Create());
        return services;
    }
}
