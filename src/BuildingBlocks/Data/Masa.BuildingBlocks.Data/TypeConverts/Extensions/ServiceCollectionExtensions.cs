// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddTypeConvert(this IServiceCollection services)
    {
        services
            .AddTypeConvertCore()
            .Configure<TypeConvertFactoryOptions>(option =>
        {
            option.TryMapping(serviceProvider => new DefaultTypeConvertProvider(serviceProvider.GetRequiredService<IDeserializerFactory>().Create()));
        });
        return services;
    }

    public static IServiceCollection AddTypeConvert(this IServiceCollection services, string name)
    {
        services
            .AddTypeConvertCore()
            .Configure<TypeConvertFactoryOptions>(option =>
        {
            option.TryMapping(name,
                serviceProvider => new DefaultTypeConvertProvider(serviceProvider.GetRequiredService<IDeserializerFactory>().Create(name)));
        });
        return services;
    }

    private static IServiceCollection AddTypeConvertCore(this IServiceCollection services)
    {
        services.TryAddSingleton<ITypeConvertFactory, DefaultTypeConvertFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ITypeConvertFactory>().Create());
        return services;
    }
}
