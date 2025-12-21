// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Data;

namespace Masa.Contrib.Extensions.BackgroundJobs.Tests;

[TestClass]
public class DefaultBackgroundJobManagerTest
{
    private Mock<IBackgroundJobStorage> _backgroundJobStorage;
    private Mock<IIdGenerator<Guid>> _idGenerator;
    private Mock<ISerializer> _serializer;
    private DefaultBackgroundJobManager _jobManager;
    private Guid _jobId = Guid.NewGuid();


    [TestInitialize]
    public void Initialize()
    {
        _backgroundJobStorage = new();
        _backgroundJobStorage.Setup(storage => storage.InsertAsync(It.IsAny<BackgroundJobInfo>())).Verifiable();

        _idGenerator = new();
        _idGenerator.Setup(generator => generator.NewId()).Returns(_jobId);

        _serializer = new();
        _serializer.Setup(s => s.Serialize(It.IsAny<object>())).Verifiable();

        _jobManager = new DefaultBackgroundJobManager(
            _backgroundJobStorage.Object,
            _idGenerator.Object,
            _serializer.Object);
    }

    [TestMethod]
    public async Task TestEnqueueAsync()
    {
        var jobId = await _jobManager.EnqueueAsync(new
        {
            account = "masa"
        });
        Assert.AreEqual(_jobId.ToString(), jobId);

        _serializer.Verify(s => s.Serialize(It.IsAny<object>()), Times.Once);
        _backgroundJobStorage.Verify(storage => storage.InsertAsync(It.IsAny<BackgroundJobInfo>()), Times.Once);
    }

    [TestMethod]
    public async Task TestAddOrUpdateScheduleAsync()
    {
        var job = new BackgroundScheduleJob()
        {
            Id = "Test",
            CronExpression = "1 * * * *"
        };
        await Assert.ThrowsExactlyAsync<BackgroundJobException>(() => _jobManager.AddOrUpdateScheduleAsync(job));
    }
}
