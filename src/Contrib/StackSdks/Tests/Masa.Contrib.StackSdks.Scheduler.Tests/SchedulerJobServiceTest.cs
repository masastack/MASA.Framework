// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Scheduler.Enum;
using Masa.BuildingBlocks.StackSdks.Scheduler.Model;
using Masa.BuildingBlocks.StackSdks.Scheduler.Request;
using Masa.Contrib.StackSdks.Scheduler;

namespace Masa.Contrib.StackSdks.Scheduler.Tests;

[TestClass]
public class SchedulerJobServiceTest
{
    const string API = "/api/scheduler-job";

    [TestMethod]
    public async Task TestAddSchedulerHttpJobAsync()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.Http,
            ProjectIdentity = "MASA_MC",
            CronExpression = "",
            HttpConfig = new SchedulerJobHttpConfig()
            {
                RequestUrl = "www.baidu.com",
                HttpVerifyType = HttpVerifyTypes.CustomStatusCode,
                HttpBody = "",
                HttpHeaders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36")
                },
                HttpMethod = HttpMethods.GET,
                VerifyContent = "200",
                HttpParameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("ie", "utf-8"),
                }
            },
            OperatorId = Guid.NewGuid(),
            Description = "Test",
            FailedRetryCount = 0,
            FailedRetryInterval = 0,
            IsAlertException = false,
            RunTimeoutSecond = 30,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.IgnoreTimeout,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Parallel,
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.Ignore
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.AddAsync(requestData);
        caller.Verify(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default), Times.Once);

        Assert.AreNotEqual<Guid>(Guid.Empty, result);
    }

    [TestMethod]
    public async Task TestAddSchedulerJobApp()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.JobApp,
            ProjectIdentity = "MASA_MC",
            CronExpression = "",
            JobAppConfig = new SchedulerJobAppConfig()
            {
                JobAppIdentity = "MASA_MC_SERVICE",
                JobEntryAssembly = "Masa.Test.Job",
                JobEntryClassName = "TestRunJob",
                JobParams = "1;2;3",
                Version = ""
            },
            OperatorId = Guid.NewGuid(),
            Description = "Test",
            FailedRetryCount = 0,
            FailedRetryInterval = 0,
            IsAlertException = false,
            RunTimeoutSecond = 30,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.IgnoreTimeout,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Parallel,
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.Ignore
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.AddAsync(requestData);
        caller.Verify(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default), Times.Once);

        Assert.AreNotEqual<Guid>(Guid.Empty, result);
    }

    [TestMethod]
    public async Task TestAddSchedulerDaprServiceInvocationJob()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.DaprServiceInvocation,
            ProjectIdentity = "MASA_MC",
            CronExpression = "",
            DaprServiceInvocationConfig = new SchedulerJobDaprServiceInvocationConfig()
            {
                DaprServiceIdentity = "MASA_MC_DAPR_SERVICE",
                Data = "Test",
                HttpMethod = HttpMethods.POST,
                MethodName = "TestMethod"
            },
            OperatorId = Guid.NewGuid(),
            Description = "Test",
            FailedRetryCount = 0,
            FailedRetryInterval = 0,
            IsAlertException = false,
            RunTimeoutSecond = 30,
            RunTimeoutStrategy = RunTimeoutStrategyTypes.IgnoreTimeout,
            ScheduleBlockStrategy = ScheduleBlockStrategyTypes.Parallel,
            ScheduleExpiredStrategy = ScheduleExpiredStrategyTypes.Ignore
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.AddAsync(requestData);
        caller.Verify(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default), Times.Once);

        Assert.AreNotEqual<Guid>(Guid.Empty, result);
    }

    [TestMethod]
    public async Task TestAddSchedulerJobArgumentNullException()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.Http,
            CronExpression = "",
            HttpConfig = new SchedulerJobHttpConfig()
            {
                RequestUrl = "www.baidu.com",
                HttpVerifyType = HttpVerifyTypes.CustomStatusCode,
                HttpBody = "",
                HttpHeaders = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/102.0.0.0 Safari/537.36")
                },
                HttpMethod = HttpMethods.GET,
                VerifyContent = "200",
                HttpParameters = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("ie", "utf-8"),
                }
            },
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await schedulerClient.SchedulerJobService.AddAsync(requestData));
    }

    [TestMethod]
    public async Task TestAddSchedulerHttpJobArgumentNullException()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.Http,
            CronExpression = "",
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await schedulerClient.SchedulerJobService.AddAsync(requestData));
    }

    [TestMethod]
    public async Task TestAddSchedulerJobAppArgumentNullException()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.JobApp,
            CronExpression = "",
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await schedulerClient.SchedulerJobService.AddAsync(requestData));
    }

    [TestMethod]
    public async Task TestAddSchedulerDaprInvocationJobArgumentNullException()
    {
        var requestData = new AddSchedulerJobRequest()
        {
            Name = "TestJob",
            JobType = JobTypes.DaprServiceInvocation,
            CronExpression = "",
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/addSchedulerJobBySdk";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PostAsync<AddSchedulerJobRequest, Guid>(requestUri, requestData, default)).ReturnsAsync(Guid.NewGuid()).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await schedulerClient.SchedulerJobService.AddAsync(requestData));
    }

    [TestMethod]
    public async Task TestRemoveSchedulerJobAsync()
    {
        var requestData = new BaseSchedulerJobRequest()
        {
            JobId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.DeleteAsync<BaseSchedulerJobRequest>(API, requestData, true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.RemoveAsync(requestData);
        caller.Verify(provider => provider.DeleteAsync<BaseSchedulerJobRequest>(API, requestData, true, default), Times.Once);

        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestStartSchedulerJobAsync()
    {
        var requestData = new BaseSchedulerJobRequest()
        {
            JobId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/startJob";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PutAsync<BaseSchedulerJobRequest>(requestUri, requestData, true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.StartAsync(requestData);
        caller.Verify(provider => provider.PutAsync<BaseSchedulerJobRequest>(requestUri, requestData, true, default), Times.Once);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestEnableSchedulerJob()
    {
        var requestData = new BaseSchedulerJobRequest()
        {
            JobId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/changeEnableStatus";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PutAsync<ChangeEnabledStatusRequest>(requestUri, It.IsAny<ChangeEnabledStatusRequest>(), true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.EnableAsync(requestData);
        caller.Verify(provider => provider.PutAsync<ChangeEnabledStatusRequest>(requestUri, It.IsAny<ChangeEnabledStatusRequest>(), true, default), Times.Once);
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task TestDisableSchedulerJob()
    {
        var requestData = new BaseSchedulerJobRequest()
        {
            JobId = Guid.NewGuid(),
            OperatorId = Guid.NewGuid()
        };

        var requestUri = $"{API}/changeEnableStatus";
        var caller = new Mock<ICaller>();
        var loggerFactory = new Mock<ILoggerFactory>();
        caller.Setup(provider => provider.PutAsync<ChangeEnabledStatusRequest>(requestUri, It.IsAny<ChangeEnabledStatusRequest>(), true, default)).Verifiable();
        var schedulerClient = new SchedulerClient(caller.Object, loggerFactory.Object);
        var result = await schedulerClient.SchedulerJobService.DisableAsync(requestData);
        caller.Verify(provider => provider.PutAsync<ChangeEnabledStatusRequest>(requestUri, It.IsAny<ChangeEnabledStatusRequest>(), true, default), Times.Once);
        Assert.IsTrue(result);
    }
}
