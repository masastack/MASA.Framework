// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Memory")]
[assembly: InternalsVisibleTo("Masa.Contrib.Extensions.BackgroundJobs.Tests")]

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Extensions.BackgroundJobs.Extensions;

internal static class BackgroundJobOptionsBuilderExtensions
{
    public static void UseBackgroundJobCore(
        this BackgroundJobOptionsBuilder backgroundJobOptionsBuilder,
        Action<BackgroundJobOptions> configure,
        Func<IServiceProvider, IIdGenerator<Guid>> idGeneratorFunc,
        Func<IServiceProvider, ISerializer> serializerFunc,
        Func<IServiceProvider, IDeserializer> deserializerFunc)
    {
        if (backgroundJobOptionsBuilder.Services.Any(s => s.ServiceType == typeof(BackgroundJobProvider)))
            return;

        backgroundJobOptionsBuilder.Services.AddSingleton<BackgroundJobProvider>();

        backgroundJobOptionsBuilder.Services.Configure(configure);
        backgroundJobOptionsBuilder.Services.TryAddSingleton<IBackgroundJobManager>(serviceProvider =>
            new DefaultBackgroundJobManager(
                serviceProvider.GetRequiredService<IBackgroundJobStorage>(),
                idGeneratorFunc.Invoke(serviceProvider),
                serializerFunc.Invoke(serviceProvider)
            ));
        if (!backgroundJobOptionsBuilder.DisableBackgroundJob)
        {
            backgroundJobOptionsBuilder.Services.TryAddSingleton<IBackgroundJobProcessor>(serviceProvider =>
                new BackgroundJobProcessor(serviceProvider, deserializerFunc.Invoke(serviceProvider)));
            backgroundJobOptionsBuilder.Services.Add(
                new ServiceDescriptor(
                    typeof(IProcessor),
                    serviceProvider => serviceProvider.GetRequiredService<IBackgroundJobProcessor>(),
                    ServiceLifetime.Singleton));
        }
        backgroundJobOptionsBuilder.Services.TryAddScoped<IProcessingServer, DefaultHostedService>();
        backgroundJobOptionsBuilder.Services.AddHostedService<BackgroundJobService>();
    }

    private sealed class BackgroundJobProvider
    {
    }
}
