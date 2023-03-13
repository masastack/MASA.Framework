// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Hangfire;

public class BackgroundJobExecutorAdapter<TArgs>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IBackgroundJobExecutor _backgroundJobExecutor;
    private readonly BackgroundJobRelationNetwork _relationNetwork;

    public BackgroundJobExecutorAdapter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _backgroundJobExecutor = _serviceProvider.GetRequiredService<IBackgroundJobExecutor>();
        _relationNetwork = _serviceProvider.GetRequiredService<BackgroundJobRelationNetwork>();
    }

    [ExcludeFromCodeCoverage]
    public async Task ExecuteAsync(TArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        var jobContext = new JobContext(scope.ServiceProvider, GetJobTypeList(), args);

        await _backgroundJobExecutor.ExecuteAsync(jobContext);
    }

    public List<Type> GetJobTypeList()
    {
        var jobName = BackgroundJobNameAttribute.GetName<TArgs>();
        return _relationNetwork.GetJobTypeList(jobName);
    }
}
