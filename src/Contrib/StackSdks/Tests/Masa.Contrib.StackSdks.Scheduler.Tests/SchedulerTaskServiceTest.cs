// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Scheduler.Request;

namespace Masa.Contrib.StackSdks.Scheduler.Tests;

[TestClass]
public class SchedulerTaskServiceTest
{
    const string API = "/api/scheduler-task";

    [TestMethod]
    public async Task TestStopSchedulerTaskAsync()
    {
        var requestData = new SchedulerTaskRequestBase()
        {
            TaskId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/stop";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync<SchedulerTaskRequestBase>(requestUri, requestData, true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object);
        var result = await schedulerClient.SchedulerTaskService.StopAsync(requestData);
        caller.Verify(provider => provider.PutAsync<SchedulerTaskRequestBase>(requestUri, requestData, true, default), Times.Once);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestStartSchedulerTaskAsync()
    {
        var request = new SchedulerTaskRequestBase()
        {
            TaskId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/start";
        var caller = new Mock<ICaller>();
        caller.Setup(provider => provider.PutAsync(requestUri, It.IsAny<StartSchedulerTaskRequest>(), true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object);
        var result = await schedulerClient.SchedulerTaskService.StartAsync(request);
        caller.Verify(provider => provider.PutAsync(requestUri, It.IsAny<StartSchedulerTaskRequest>(), true, default), Times.Once);
        Assert.IsTrue(result);
    }
}
