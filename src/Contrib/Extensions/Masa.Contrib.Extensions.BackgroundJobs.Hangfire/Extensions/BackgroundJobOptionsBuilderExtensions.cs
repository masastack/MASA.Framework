// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public static class BackgroundJobOptionsBuilderExtensions
{
    public static void UseHangfire(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<IGlobalConfiguration> configuration)
    {
        backgroundJobOptionsBuilder.UseHangfireCore(services => services.AddHangfire(configuration).AddHangfireServer());
    }

    public static void UseHangfire(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<IServiceProvider, IGlobalConfiguration> configuration)
    {
        backgroundJobOptionsBuilder.UseHangfireCore(services => services.AddHangfire(configuration).AddHangfireServer());
    }

    private static void UseHangfireCore(this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder, Action<IServiceCollection> configure)
    {
        if (backgroundJobOptionsBuilder.Services.Any(s => s.ServiceType == typeof(BackgroundJobProvider)))
            return;

        backgroundJobOptionsBuilder.Services.AddSingleton<BackgroundJobProvider>();

        backgroundJobOptionsBuilder.Services.TryAddSingleton<IBackgroundJobManager, DefaultBackgroundJobManager>();
        configure.Invoke(backgroundJobOptionsBuilder.Services);
    }

#pragma warning disable S2094
    private sealed class BackgroundJobProvider
    {
    }
#pragma warning restore S2094
}
