// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs;

public class DefaultBackgroundJobManager : IBackgroundJobManager
{
    private readonly IBackgroundJobStorage _backgroundJobStorage;
    private readonly IIdGenerator<Guid> _idGenerator;
    private readonly ISerializer _serializer;

    public DefaultBackgroundJobManager(
        IBackgroundJobStorage backgroundJobStorage,
        IIdGenerator<Guid> idGenerator,
        ISerializer serializer)
    {
        _backgroundJobStorage = backgroundJobStorage;
        _idGenerator = idGenerator;
        _serializer = serializer;
    }

    public async Task<string> EnqueueAsync<TArgs>(TArgs args, TimeSpan? delay = null)
    {
        var dateTimeNow = DateTime.UtcNow;
        var jobInfo = new BackgroundJobInfo()
        {
            Id = _idGenerator.NewId(),
            Name = BackgroundJobNameAttribute.GetName<TArgs>(),
            Args = _serializer.Serialize(args),
            CreationTime = dateTimeNow,
            NextTryTime = delay == null ? dateTimeNow : dateTimeNow.Add(delay.Value)
        };
        await _backgroundJobStorage.InsertAsync(jobInfo);
        return jobInfo.Id.ToString();
    }

    public virtual Task AddOrUpdateScheduleAsync(IBackgroundScheduleJob backgroundScheduleJob)
        => throw new BackgroundJobException(errorCode: ExceptionErrorCode.NOT_SUPPORT_PERIODICALLY_JOB);
}
