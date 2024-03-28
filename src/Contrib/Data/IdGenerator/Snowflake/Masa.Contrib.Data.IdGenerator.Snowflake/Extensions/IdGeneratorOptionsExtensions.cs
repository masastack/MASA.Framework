// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Data.IdGenerator.Snowflake.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public static class IdGeneratorOptionsExtensions
{
    public static void UseSnowflakeGenerator(this IdGeneratorOptions options)
        => options.UseSnowflakeGenerator(null);

    public static void UseSnowflakeGenerator(this IdGeneratorOptions options, Action<SnowflakeGeneratorOptions>? action)
    {

#if (NET8_0 || NET8_0_OR_GREATER)
        if (options.Services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(SnowflakeGeneratorProvider))) return;
#else
        if (options.Services.Any(service => service.ImplementationType == typeof(SnowflakeGeneratorProvider))) return;
#endif

        options.Services.AddSingleton<SnowflakeGeneratorProvider>();

        UseSnowflakeGeneratorCore(options.Services, action);
    }

    private static void UseSnowflakeGeneratorCore(IServiceCollection services, Action<SnowflakeGeneratorOptions>? action)
    {
        var snowflakeGeneratorOptions = new SnowflakeGeneratorOptions(services);
        action?.Invoke(snowflakeGeneratorOptions);

        services.TryAddSingleton<IWorkerProvider, DefaultWorkerProvider>();

        CheckIdGeneratorOptions(services, snowflakeGeneratorOptions);

        if (snowflakeGeneratorOptions.EnableMachineClock)
        {
            services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new MachineClockIdGenerator(serviceProvider.GetRequiredService<IWorkerProvider>(), snowflakeGeneratorOptions));
        }
        else
        {
            services.TryAddSingleton<ISnowflakeGenerator>(serviceProvider
                => new SnowflakeIdGenerator(serviceProvider.GetRequiredService<IWorkerProvider>(),
                    snowflakeGeneratorOptions));
        }

        services.TryAddSingleton<IIdGenerator<long>>(serviceProvider => serviceProvider.GetRequiredService<ISnowflakeGenerator>());
        services.TryAddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<ISnowflakeGenerator>());

        if (snowflakeGeneratorOptions.SupportDistributed)
        {
            services.Add(ServiceDescriptor.Singleton<IHostedService>(serviceProvider
                => new WorkerIdBackgroundServices(
                    snowflakeGeneratorOptions.HeartbeatInterval,
                    snowflakeGeneratorOptions.MaxExpirationTime,
                    serviceProvider.GetRequiredService<IWorkerProvider>(),
                    serviceProvider.GetService<ILogger<WorkerIdBackgroundServices>>()
                )));
        }
    }

    internal static void CheckIdGeneratorOptions(IServiceCollection services, SnowflakeGeneratorOptions generatorOptions)
    {
        if (generatorOptions.BaseTime > DateTime.UtcNow)
            throw new ArgumentOutOfRangeException(nameof(generatorOptions), $"{nameof(generatorOptions.BaseTime)} must not be greater than the current time");

        if (generatorOptions.SupportDistributed)
        {
            if (generatorOptions.HeartbeatInterval < 100)
                throw new ArgumentOutOfRangeException(nameof(generatorOptions), $"{nameof(generatorOptions.HeartbeatInterval)} must be greater than 100");
        }
        else
        {
            long workerId = GetInstance<IWorkerProvider>(services).GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (workerId > generatorOptions.MaxWorkerId)
                throw new ArgumentOutOfRangeException(nameof(generatorOptions), $"workerId must be greater than 0 or less than or equal to {generatorOptions.MaxWorkerId}");
        }

        var maxLength = generatorOptions.TimestampType == TimestampType.Milliseconds ? 22 : 31;
        if (generatorOptions.SequenceBits + generatorOptions.WorkerIdBits > maxLength)
            throw new ArgumentOutOfRangeException(
                $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than {maxLength}");
    }

    private static TService GetInstance<TService>(this IServiceCollection services) where TService : notnull =>
        services.BuildServiceProvider().GetRequiredService<TService>();

#pragma warning disable S2094
    private sealed class SnowflakeGeneratorProvider
    {
    }
#pragma warning restore S2094
}
