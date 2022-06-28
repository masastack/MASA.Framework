// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Scheduler.Tests;

[TestClass]
public class SchedulerTaskServiceTest
{
    const string API = "/api/scheduler-task";

    [TestMethod]
    public async Task TestStopSchedulerTaskAsync()
    {
        var requestData = new BaseSchedulerTaskRequest()
        {
            TaskId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/stop";
        var callerProvider = new Mock<ICallerProvider>();
        var loggerFactory = new Mock<ILoggerFactory>();
        callerProvider.Setup(provider => provider.PutAsync<BaseSchedulerTaskRequest>(requestUri, requestData, true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(callerProvider.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerTaskService.StopSchedulerTaskAsync(requestData);
        callerProvider.Verify(provider => provider.PutAsync<BaseSchedulerTaskRequest>(requestUri, requestData, true, default), Times.Once);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestStartSchedulerTaskAsync()
    {
        var request = new BaseSchedulerTaskRequest()
        {
            TaskId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/start";
        var callerProvider = new Mock<ICallerProvider>();
        var loggerFactory = new Mock<ILoggerFactory>();
        callerProvider.Setup(provider => provider.PutAsync(requestUri, It.IsAny<StartSchedulerTaskRequest>(), true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(callerProvider.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerTaskService.StartSchedulerTaskAsync(request);
        callerProvider.Verify(provider => provider.PutAsync(requestUri, It.IsAny<StartSchedulerTaskRequest>(), true, default), Times.Once);
        Assert.IsTrue(result);
    }
}
