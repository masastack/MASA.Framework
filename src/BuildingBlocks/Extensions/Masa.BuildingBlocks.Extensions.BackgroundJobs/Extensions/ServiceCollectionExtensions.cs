// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundJob(
        this IServiceCollection services,
        Action<BackgroundJobOptionsBuilder> configure)
    {
        MasaApp.TrySetServiceCollection(services);
        var backgroundJobOptionsBuilder = new BackgroundJobOptionsBuilder(services);
        configure.Invoke(backgroundJobOptionsBuilder);

        services.TryAddSingleton(
            new BackgroundJobRelationNetwork(
                services,
                backgroundJobOptionsBuilder.Assemblies ?? MasaApp.GetAssemblies()).Build());
        services.TryAddSingleton<IBackgroundJobExecutor, DefaultBackgroundJobExecutor>();

        return services;
    }

    public static IServiceCollection AddBackgroundJobServer(this IServiceCollection services)
    {
        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();
        services.AddHostedService<BackgroundJobService>();
        return services;
    }
}
