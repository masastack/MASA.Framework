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

        services.TryAddSingleton<IWorkerProvider, DefaultWorkerProvider>();

        CheckIdGeneratorOptions(idGeneratorOptions,
            services.GetInstance<IWorkerProvider>().GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult());

        services.TryAddSingleton<IIdGenerator<System.Snowflake, long>>(serviceProvider
            => serviceProvider.GetRequiredService<ISnowflakeGenerator>());
        if (idGeneratorOptions.EnableMachineClock)
        {
            services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IWorkerProvider>(), idGeneratorOptions));
        }
        else
        {
            services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new SnowflakeIdGenerator(serviceProvider.GetRequiredService<IWorkerProvider>(),
                    idGeneratorOptions));
        }

        if (idGeneratorOptions.SupportDistributed)
        {
            services.Add(ServiceDescriptor.Singleton<IHostedService>(serviceProvider
                => new WorkerIdBackgroundServices(
                    idGeneratorOptions.HeartbeatInterval,
                    idGeneratorOptions.MaxExpirationTime,
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    serviceProvider.GetService<ILogger<WorkerIdBackgroundServices>>()
                )));
        }
        IdGeneratorFactory.SetSnowflakeGenerator(services.BuildServiceProvider().GetRequiredService<ISnowflakeGenerator>());
        return services;
    }

    private static TService GetInstance<TService>(this IServiceCollection services) where TService : notnull =>
        services.BuildServiceProvider().GetRequiredService<TService>();

    private static void CheckIdGeneratorOptions(IdGeneratorOptions generatorOptions, long workerId)
    {
        if (generatorOptions.BaseTime > DateTime.UtcNow)
            throw new ArgumentOutOfRangeException(nameof(generatorOptions.BaseTime),
                $"{nameof(generatorOptions.BaseTime)} must not be greater than the current time");

        if (workerId > generatorOptions.MaxWorkerId)
            throw new ArgumentOutOfRangeException(
                $"workerId must be greater than 0 or less than or equal to {generatorOptions.MaxWorkerId}");

        var maxLength = generatorOptions.TimestampType == TimestampType.Milliseconds ? 22 : 31;
        if (generatorOptions.SequenceBits + generatorOptions.WorkerIdBits > maxLength)
            throw new ArgumentOutOfRangeException(
                $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than {maxLength}");

        if (generatorOptions.SupportDistributed && generatorOptions.HeartbeatInterval < 100)
            throw new ArgumentOutOfRangeException($"{nameof(generatorOptions.HeartbeatInterval)} must be greater than 100");
    }
}
