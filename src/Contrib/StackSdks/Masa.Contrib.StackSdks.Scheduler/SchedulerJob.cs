// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.StackSdks.Scheduler;

public abstract class SchedulerJob : ISchedulerJob
{
    protected SchedulerLogger<SchedulerJob> Logger { get; set; }

    protected WebApplicationBuilder Builder { get; set; }

    protected IServiceProvider ServiceProvider { get; set; }

    public Task Init(WebApplicationBuilder builder, IServiceProvider serviceProvider, Guid jobId, Guid taskId)
    {
        Builder = builder;
        ServiceProvider = serviceProvider;
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        Logger = new SchedulerLogger<SchedulerJob>(loggerFactory, jobId, taskId);
        return Task.CompletedTask;
    }

    public virtual Task AfterExcuteAsync(JobContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task BeforeExcuteAsync(JobContext context)
    {
        return Task.CompletedTask;
    }

    public abstract Task<object?> ExcuteAsync(JobContext context);
}
