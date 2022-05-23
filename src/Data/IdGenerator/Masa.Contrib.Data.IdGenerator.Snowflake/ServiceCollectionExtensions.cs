// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnowflake(this IServiceCollection services)
        => services.AddSnowflake(null);

    public static IServiceCollection AddSnowflake(this IServiceCollection services, Action<IdGeneratorOptions>? options)
    {
        var idGeneratorOptions = new IdGeneratorOptions();
        options?.Invoke(idGeneratorOptions);
        CheckIdGeneratorOptions(idGeneratorOptions);

        services.TryAddSingleton<IIdGenerator>(_ => new DefaultIdGenerator(idGeneratorOptions));
        return services;
    }

    private static void CheckIdGeneratorOptions(IdGeneratorOptions generatorOptions)
    {
        if (generatorOptions.BaseTime > DateTime.Now)
            throw new MasaException($"{nameof(generatorOptions.BaseTime)} must not be greater than the current time");

        if (generatorOptions.WorkerId > generatorOptions.MaxWorkerId || generatorOptions.WorkerId < 0)
            throw new ArgumentException($"worker Id can't be greater than {generatorOptions.MaxWorkerId} or less than 0");

        if (generatorOptions.DatacenterId > generatorOptions.MaxDatacenterId || generatorOptions.DatacenterId < 0)
            throw new ArgumentException($"datacenter Id can't be greater than {generatorOptions.MaxDatacenterId} or less than 0");
    }
}
