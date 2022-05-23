// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Yitter;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnowflake(this IServiceCollection services)
        => services.AddSnowflake(null);

    public static IServiceCollection AddSnowflake(this IServiceCollection services, Action<IdGeneratorOptions>? options)
    {
        var idGeneratorOptions = new IdGeneratorOptions(1);
        options?.Invoke(idGeneratorOptions);
        services.TryAddSingleton<IIdGenerator>(_ => new IdGenerator(idGeneratorOptions));
        return services;
    }
}
