// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.TypeConverts;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTypeConvert(this IServiceCollection services)
    {
        services.TryAddSingleton<ITypeConvertFactory, DefaultTypeConvertFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ITypeConvertFactory>().Create());
        services.Configure<TypeConvertOptions>(option =>
        {
            string name = DataType.Json.ToString();
            option.Mapping(name,
                serviceProvider => new DefaultTypeConvertProvider(serviceProvider.GetRequiredService<IDeserializerFactory>().Create(name)));
        });
        return services;
    }
}
