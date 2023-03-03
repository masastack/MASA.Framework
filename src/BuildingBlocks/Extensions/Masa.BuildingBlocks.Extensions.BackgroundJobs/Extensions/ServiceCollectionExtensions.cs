// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs.Extensions;

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

        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();
        services.AddHostedService<BackgroundJobService>();
        return services;
    }
}
