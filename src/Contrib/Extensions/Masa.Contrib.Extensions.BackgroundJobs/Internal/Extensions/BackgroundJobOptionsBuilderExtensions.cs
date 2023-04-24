﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Memory")]
[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Extensions.BackgroundJobs;

internal static class BackgroundJobOptionsBuilderExtensions
{
    public static void UseBackgroundJobCore(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<BackgroundJobOptions> configure,
        Func<IServiceProvider, IDeserializer> deserializerFunc)
    {
        if (backgroundJobOptionsBuilder.Services.Any(s => s.ServiceType == typeof(BackgroundJobProvider)))
            return;

        backgroundJobOptionsBuilder.Services.AddSingleton<BackgroundJobProvider>();

        backgroundJobOptionsBuilder.Services.Configure(configure);
        backgroundJobOptionsBuilder.Services.TryAddSingleton<IBackgroundJobProcessor>(serviceProvider =>
            new BackgroundJobProcessor(serviceProvider, deserializerFunc.Invoke(serviceProvider)));
        backgroundJobOptionsBuilder.Services.Add(
            new ServiceDescriptor(
                typeof(IProcessor),
                serviceProvider => serviceProvider.GetRequiredService<IBackgroundJobProcessor>(),
                ServiceLifetime.Singleton));
    }

#pragma warning disable S2094
    private sealed class BackgroundJobProvider
    {
    }
#pragma warning restore S2094
}
