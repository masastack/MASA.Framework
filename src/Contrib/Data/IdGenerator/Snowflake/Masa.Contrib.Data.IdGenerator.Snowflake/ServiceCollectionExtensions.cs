// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSnowflake(this IServiceCollection services)
        => services.AddSnowflake(null);

    public static IServiceCollection AddSnowflake(this IServiceCollection services, Action<SnowflakeGeneratorOptions>? action)
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
        services.AddSingleton<IIdGenerator<long>>(serviceProvider => serviceProvider.GetRequiredService<ISnowflakeGenerator>());
        services.AddSingleton<IIdGenerator>(serviceProvider => serviceProvider.GetRequiredService<ISnowflakeGenerator>());

        services.Configure<IdGeneratorFactoryOptions>(factoryOptions =>
        {
            factoryOptions.Options.Add(new IdGeneratorRelationOptions(snowflakeGeneratorOptions.Name)
            {
                Func = serviceProvider => serviceProvider.GetRequiredService<IGuidGenerator>()
            });
        });

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
        return services;
    }

    private static TService GetInstance<TService>(this IServiceCollection services) where TService : notnull =>
        services.BuildServiceProvider().GetRequiredService<TService>();

    private static void CheckIdGeneratorOptions(IServiceCollection services, SnowflakeGeneratorOptions generatorOptions)
    {
        if (generatorOptions.BaseTime > DateTime.UtcNow)
            throw new ArgumentOutOfRangeException(nameof(generatorOptions.BaseTime),
                $"{nameof(generatorOptions.BaseTime)} must not be greater than the current time");

        if (generatorOptions.SupportDistributed)
        {
            if (generatorOptions.HeartbeatInterval < 100)
                throw new ArgumentOutOfRangeException($"{nameof(generatorOptions.HeartbeatInterval)} must be greater than 100");
        }
        else
        {
            long workerId = GetInstance<IWorkerProvider>(services).GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            if (workerId > generatorOptions.MaxWorkerId)
                throw new ArgumentOutOfRangeException(
                    $"workerId must be greater than 0 or less than or equal to {generatorOptions.MaxWorkerId}");
        }

        var maxLength = generatorOptions.TimestampType == TimestampType.Milliseconds ? 22 : 31;
        if (generatorOptions.SequenceBits + generatorOptions.WorkerIdBits > maxLength)
            throw new ArgumentOutOfRangeException(
                $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than {maxLength}");
    }
}
