// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Extensions.BackgroundJobs;

public abstract class BackgroundJobBase<TArgs> : IBackgroundJob<TArgs>
{
    public string? JobName => null;

    protected readonly ILogger<BackgroundJobBase<TArgs>>? Logger;

    protected BackgroundJobBase(ILogger<BackgroundJobBase<TArgs>>? logger)
    {
        Logger = logger;
    }

    protected virtual Task PreExecuteAsync(TArgs args)
    {
        Logger?.LogDebug("----- background task running and jobArgs: {JobArgs}", args.ToJson());
        return Task.CompletedTask;
    }

    public async Task ExecuteAsync(TArgs args)
    {
        await PreExecuteAsync(args);
        await ExecutingAsync(args);
        await PostExecuteAsync(args);
    }

    protected abstract Task ExecutingAsync(TArgs args);

    protected virtual Task PostExecuteAsync(TArgs args)
    {
        Logger?.LogDebug("-----The end of the background task, jobArgs: {JobArgs}", args.ToJson());
        return Task.CompletedTask;
    }
}
