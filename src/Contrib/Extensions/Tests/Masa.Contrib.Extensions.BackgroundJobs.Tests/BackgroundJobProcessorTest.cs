// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Tests;

[TestClass]
public class BackgroundJobProcessorTest
{
    private IServiceCollection _services;
    private readonly int _batchSize = 2;
    private RegisterAccountParameter _parameter;

    [TestInitialize]
    public void Initialize()
    {
        _services = new ServiceCollection();
        _services.Configure<BackgroundJobOptions>(options =>
        {
            options.BatchSize = _batchSize;
        });
        _services.AddSingleton(new BackgroundJobRelationNetwork(_services, AppDomain.CurrentDomain.GetAssemblies()));
        _parameter = new RegisterAccountParameter()
        {
            Account = "masastack"
        };
    }

    [TestMethod]
    public async Task TestBackgroundJobProcessorAsync()
    {
        Mock<IBackgroundJobStorage> backgroundJobStorage = new();
        var jobs = new List<BackgroundJobInfo>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = typeof(RegisterAccountParameter).FullName!,
                Args = System.Text.Json.JsonSerializer.Serialize(_parameter)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = typeof(RegisterAccountParameter).FullName!,
                Args = System.Text.Json.JsonSerializer.Serialize(_parameter)
            }
        };
        backgroundJobStorage.Setup(storage => storage.RetrieveJobsAsync(It.IsAny<int>())).ReturnsAsync(() => jobs);
        _services.AddSingleton(backgroundJobStorage.Object);
        backgroundJobStorage.Setup(storage => storage.DeleteAsync(It.IsAny<Guid>())).Verifiable();

        Mock<IBackgroundJobExecutor> backgroundJobExecutor = new();
        backgroundJobExecutor.Setup(executor => executor.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>())).Verifiable();
        _services.AddSingleton(backgroundJobExecutor.Object);

        Mock<IDeserializer> deserializer = new();
        deserializer.Setup(d => d.Deserialize(It.IsAny<string>(), typeof(RegisterAccountParameter))).Returns(() => _parameter);

        var serviceProvider = _services.BuildServiceProvider();
        var backgroundJobProcessor = new BackgroundJobProcessor(serviceProvider, deserializer.Object);
        var methodInfo = typeof(BackgroundJobProcessor).GetMethod("ExecuteJobAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;
        var result = methodInfo.Invoke(backgroundJobProcessor, new object[]
        {
            CancellationToken.None
        }) as Task;
        await result!;
        backgroundJobStorage.Verify(s => s.RetrieveJobsAsync(It.IsAny<int>()), Times.Once);
        backgroundJobExecutor.Verify(s => s.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        backgroundJobStorage.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Exactly(2));
        backgroundJobStorage.Verify(s => s.UpdateAsync(It.IsAny<BackgroundJobInfo>()), Times.Never);
    }

    [DataRow(29)]
    [DataRow(30)]
    [DataTestMethod]
    public async Task TestBackgroundJobProcessorBySendCouponAsync(int times)
    {
        Mock<IBackgroundJobStorage> backgroundJobStorage = new();
        var jobs = new List<BackgroundJobInfo>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = typeof(SendCouponParameter).FullName!,
                Args = System.Text.Json.JsonSerializer.Serialize(new SendCouponParameter()
                {
                    Account = "masa"
                }),
                Times = times
            }
        };
        backgroundJobStorage.Setup(storage => storage.RetrieveJobsAsync(It.IsAny<int>())).ReturnsAsync(() => jobs);
        _services.AddSingleton(backgroundJobStorage.Object);
        backgroundJobStorage.Setup(storage => storage.DeleteAsync(It.IsAny<Guid>())).Verifiable();

        Mock<IBackgroundJobExecutor> backgroundJobExecutor = new();
        backgroundJobExecutor.Setup(executor => executor.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>())).Verifiable();
        _services.AddSingleton(backgroundJobExecutor.Object);

        Mock<IDeserializer> deserializer = new();
        deserializer.Setup(d => d.Deserialize(It.IsAny<string>(), typeof(RegisterAccountParameter))).Returns(() => _parameter);

        var serviceProvider = _services.BuildServiceProvider();
        var backgroundJobProcessor = new BackgroundJobProcessor(serviceProvider, deserializer.Object);
        var methodInfo = typeof(BackgroundJobProcessor).GetMethod("ExecuteJobAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var result = methodInfo.Invoke(backgroundJobProcessor, new object[]
        {
            CancellationToken.None
        }) as Task;
        await result!;

        backgroundJobStorage.Verify(s => s.RetrieveJobsAsync(It.IsAny<int>()), Times.Once);
        backgroundJobExecutor.Verify(s => s.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>()), Times.Never);
        backgroundJobStorage.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        backgroundJobStorage.Verify(s => s.UpdateAsync(It.IsAny<BackgroundJobInfo>()), Times.Once);
    }

    [DataRow(29)]
    [DataRow(30)]
    [DataTestMethod]
    public async Task TestBackgroundJobProcessorByExecuteFailedAsync(int times)
    {
        Mock<IBackgroundJobStorage> backgroundJobStorage = new();
        var jobs = new List<BackgroundJobInfo>()
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = typeof(RegisterAccountParameter).FullName!,
                Args = System.Text.Json.JsonSerializer.Serialize(_parameter),
                Times = times
            }
        };
        backgroundJobStorage.Setup(storage => storage.RetrieveJobsAsync(It.IsAny<int>())).ReturnsAsync(() => jobs);
        _services.AddSingleton(backgroundJobStorage.Object);
        backgroundJobStorage.Setup(storage => storage.DeleteAsync(It.IsAny<Guid>())).Verifiable();

        Mock<IBackgroundJobExecutor> backgroundJobExecutor = new();
        backgroundJobExecutor.Setup(executor => executor.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>())).Throws(new NotSupportedException());
        _services.AddSingleton(backgroundJobExecutor.Object);

        Mock<IDeserializer> deserializer = new();
        deserializer.Setup(d => d.Deserialize(It.IsAny<string>(), typeof(RegisterAccountParameter))).Returns(() => _parameter);

        var serviceProvider = _services.BuildServiceProvider();
        var backgroundJobProcessor = new BackgroundJobProcessor(serviceProvider, deserializer.Object);
        var methodInfo = typeof(BackgroundJobProcessor).GetMethod("ExecuteJobAsync", BindingFlags.Instance | BindingFlags.NonPublic)!;

        var result = methodInfo.Invoke(backgroundJobProcessor, new object[]
        {
            CancellationToken.None
        }) as Task;
        await result!;

        backgroundJobStorage.Verify(s => s.RetrieveJobsAsync(It.IsAny<int>()), Times.Once);
        backgroundJobExecutor.Verify(s => s.ExecuteAsync(It.IsAny<JobContext>(), It.IsAny<CancellationToken>()), Times.Once);
        backgroundJobStorage.Verify(s => s.DeleteAsync(It.IsAny<Guid>()), Times.Never);
        backgroundJobStorage.Verify(s => s.UpdateAsync(It.IsAny<BackgroundJobInfo>()), Times.Once);
    }
}
