// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static partial class ServiceCollectionExtensions
{
    public static void TryAddSerializationCore(this IServiceCollection services)
    {
        services.TryAddSingleton<IDeserializerFactory, DefaultDeserializerFactory>();
        services.TryAddSingleton<ISerializerFactory, DefaultSerializerFactory>();
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<ISerializerFactory>().Create());
        services.TryAddSingleton(serviceProvider => serviceProvider.GetRequiredService<IDeserializerFactory>().Create());
    }
}
