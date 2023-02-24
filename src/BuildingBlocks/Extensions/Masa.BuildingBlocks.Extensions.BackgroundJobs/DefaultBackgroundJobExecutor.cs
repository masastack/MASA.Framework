// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class DefaultBackgroundJobExecutor : IBackgroundJobExecutor
{
    private readonly ILogger<DefaultBackgroundJobExecutor>? _logger;
    private readonly IOptions<BackgroundJobRelationOptions> _backgroundJobRelationOptions;

    public DefaultBackgroundJobExecutor(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<DefaultBackgroundJobExecutor>>();
        _backgroundJobRelationOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobRelationOptions>>();
        var backgroundJobOptions = serviceProvider.GetRequiredService<IOptions<BackgroundJobOptions>>().Value;
        _backgroundJobRelationOptions.Value.Initialize(backgroundJobOptions.Assemblies ?? MasaApp.GetAssemblies());
    }

    public Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
    {
        var job = context.ServiceProvider.GetService(context.Type);
        MasaArgumentException.ThrowIfNull(job);

        var parameters = new[]
        {
            context.Args,
            cancellationToken
        };
        try
        {
            var taskInvokeDelegate = _backgroundJobRelationOptions.Value.GetInvokeDelegate(context.Type);
            return taskInvokeDelegate.Invoke(job, parameters);
        }
        catch (Exception ex)
        {
            _logger?.LogWarning(ex,
                "A background job execution is failed. JobType: {Type}, JobArgs: {Args}.",
                context.Type.FullName ?? context.Type.Name,
                context.Args);
            throw;
        }
    }
}
