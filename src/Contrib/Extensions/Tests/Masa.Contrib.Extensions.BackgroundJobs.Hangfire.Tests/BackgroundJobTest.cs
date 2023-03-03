// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Memory;

[TestClass]
public class BackgroundJobTest
{
    [TestInitialize]
    public void Initialize()
    {
        BackgroundJobManager.ResetBackgroundJobManager();
        MasaApp.SetServiceCollection(new ServiceCollection());
    }

    [TestMethod]
    public void TestBackgroundJobByUseHangfire()
    {
        var services = new ServiceCollection();
        services.AddBackgroundJob(jobBuilder => jobBuilder.UseHangfire(_ =>
        {

        }));
        var serviceProvider = services.BuildServiceProvider();
        var backgroundJobManagers = serviceProvider.GetServices<IBackgroundJobManager>();
        Assert.AreEqual(1, backgroundJobManagers.Count());
        Assert.IsNotNull(serviceProvider.GetService<IGlobalConfiguration>());
    }

    [TestMethod]
    public void TestBackgroundJobByUseHangfire2()
    {
        var services = new ServiceCollection();
        services.AddBackgroundJob(jobBuilder => jobBuilder.UseHangfire((_, _) =>
        {

        }));
        var serviceProvider = services.BuildServiceProvider();
        var backgroundJobManagers = serviceProvider.GetServices<IBackgroundJobManager>();
        Assert.AreEqual(1, backgroundJobManagers.Count());
        Assert.IsNotNull(serviceProvider.GetService<IGlobalConfiguration>());
    }

    [TestMethod]
    public async Task TestBackgroundJobManagerAsync()
    {
        var services = new ServiceCollection();
        Mock<IBackgroundJobManager> backgroundJobManager = new();
        backgroundJobManager.Setup(m => m.EnqueueAsync(It.IsAny<object>(), It.IsAny<TimeSpan?>())).Verifiable();
        backgroundJobManager.Setup(m => m.AddOrUpdateScheduleAsync(It.IsAny<IBackgroundScheduleJob>())).Verifiable();
        services.AddSingleton(backgroundJobManager.Object);
        MasaApp.SetServiceCollection(services);

        await BackgroundJobManager.EnqueueAsync(new
        {
            Account = "masa"
        });
        backgroundJobManager.Verify(m => m.EnqueueAsync(It.IsAny<object>(), It.IsAny<TimeSpan?>()), Times.Once);

        await BackgroundJobManager.AddOrUpdateScheduleAsync(new HangfireBackgroundScheduleJob());
        backgroundJobManager.Verify(m => m.AddOrUpdateScheduleAsync(It.IsAny<IBackgroundScheduleJob>()), Times.Once);
    }

    [TestMethod]
    public async Task TestBackgroundJobManagerByEmptyServicesAsync()
    {
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => BackgroundJobManager.EnqueueAsync(new
        {
            Account = "masa"
        }));
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(()
            => BackgroundJobManager.AddOrUpdateScheduleAsync(new HangfireBackgroundScheduleJob()));
    }

    [TestMethod]
    public void TestBackgroundJobExecutorAdapter()
    {
        var services = new ServiceCollection();
        services.AddBackgroundJob(jobBuilder => jobBuilder.UseHangfire(_ =>
        {

        }));
        var serviceProvider = services.BuildServiceProvider();
        var backgroundJobExecutorAdapter = new BackgroundJobExecutorAdapter<RegisterAccountParameter>(serviceProvider);
        var jobTypeList = backgroundJobExecutorAdapter.GetJobTypeList();
        Assert.AreEqual(1, jobTypeList.Count());
        Assert.AreEqual(typeof(RegisterAccountBackgroundJob), jobTypeList[0]);
    }
}
