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
        idGeneratorOptions.WorkerId =
            services.GetInstance<IWorkerProvider>().GetWorkerIdAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        CheckIdGeneratorOptions(idGeneratorOptions);

        services.TryAddSingleton<ITimeCallbackProvider, EmptyTimeCallbackProvider>();

        if (idGeneratorOptions.EnableMachineClock)
        {
            services.TryAddSingleton<IIdGenerator>(_ => new MachineClockIdGenerator(idGeneratorOptions));
        }
        else
        {
            services.TryAddSingleton<IIdGenerator>(serviceProvider
                => new DefaultIdGenerator(serviceProvider.GetRequiredService<ITimeCallbackProvider>(),
                    idGeneratorOptions));
        }

        services.Add(ServiceDescriptor.Singleton<IHostedService>(serviceProvider
            => new WorkerIdBackgroundServices(serviceProvider.GetRequiredService<IWorkerProvider>())));
        return services;
    }

    private static TService GetInstance<TService>(this IServiceCollection services) where TService : notnull =>
        services.BuildServiceProvider().GetRequiredService<TService>();

    private static void CheckIdGeneratorOptions(IdGeneratorOptions generatorOptions)
    {
        if (generatorOptions.BaseTime > DateTime.Now)
            throw new MasaException($"{nameof(generatorOptions.BaseTime)} must not be greater than the current time");

        if (generatorOptions.WorkerId > generatorOptions.MaxWorkerId)
            throw new ArgumentException(
                $"{nameof(generatorOptions.WorkerId)} must be greater than 0 or less than or equal to {generatorOptions.MaxWorkerId}");

        if (generatorOptions.SequenceBits + generatorOptions.WorkerIdBits > 22)
            throw new ArgumentNullException(
                $"The sum of {nameof(generatorOptions.WorkerIdBits)} And {nameof(generatorOptions.SequenceBits)} must be less than 22");
    }
}
