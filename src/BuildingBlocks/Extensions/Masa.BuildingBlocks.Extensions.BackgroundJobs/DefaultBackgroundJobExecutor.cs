// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public class DefaultBackgroundJobExecutor : IBackgroundJobExecutor
{
    private readonly ILogger<DefaultBackgroundJobExecutor>? _logger;
    private readonly BackgroundJobRelationNetwork _backgroundJobRelationNetwork;

    public DefaultBackgroundJobExecutor(IServiceProvider serviceProvider)
    {
        _logger = serviceProvider.GetService<ILogger<DefaultBackgroundJobExecutor>>();
        _backgroundJobRelationNetwork = serviceProvider.GetRequiredService<BackgroundJobRelationNetwork>();
    }

    public async Task ExecuteAsync(JobContext context, CancellationToken cancellationToken = default)
    {
        foreach (var jobType in context.Types)
        {
            await ExecuteAsync(context.ServiceProvider, jobType, context.Args, cancellationToken);
        }
    }

    private Task ExecuteAsync(
        IServiceProvider serviceProvider,
        Type jobType,
        object? jobArgs,
        CancellationToken cancellationToken = default)
    {
        var job = serviceProvider.GetService(jobType);
        MasaArgumentException.ThrowIfNull(job);

        var parameters = new[]
        {
            jobArgs, cancellationToken
        };
        try
        {
            var taskInvokeDelegate = _backgroundJobRelationNetwork.GetInvokeDelegate(jobType);
            return taskInvokeDelegate.Invoke(job, parameters);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex,
                "----- A background job execution is failed. JobType: {Type}, JobArgs: {Args}.",
                jobType.FullName ?? jobType.Name,
                jobArgs.ToJson());
            throw;
        }
    }
}
