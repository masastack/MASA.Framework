// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundJob(
        this IServiceCollection services,
        Action<BackgroundJobOptionsBuilder> configure,
        Action<BackgroundJobOptions>? backgroundJobOptions = null)
    {
        configure.Invoke(new(services));
        if (backgroundJobOptions != null)
        {
            services.Configure(backgroundJobOptions);
            services.Configure<BackgroundJobOptions>(options =>
            {
                options.DisableBackgroundJob = false;
                options.Assemblies = MasaApp.GetAssemblies();
            });
        }
        return services;
    }
}
