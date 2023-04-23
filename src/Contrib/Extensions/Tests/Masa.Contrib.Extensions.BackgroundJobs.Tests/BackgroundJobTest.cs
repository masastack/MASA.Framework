// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Extensions.BackgroundJobs.Tests;

[TestClass]
public class BackgroundJobTest
{
    [TestMethod]
    public void TestUseBackgroundJob()
    {
        var services = new ServiceCollection();
        Mock<IDeserializer> deserializer = new();

        services.AddBackgroundJob(jobBuilder =>
        {
            jobBuilder.UseBackgroundJobCore(_ => { },
                _ => deserializer.Object);
        });

        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IBackgroundJobProcessor) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IProcessor) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IProcessingServer) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IHostedService) && s.Lifetime == ServiceLifetime.Singleton));

        var serviceProvider = services.BuildServiceProvider();
        var processors = serviceProvider.GetServices<IProcessor>();
        Assert.AreEqual(1, processors.Count());
    }

    [TestMethod]
    public void TestUseBackgroundJobByDisableBackgroundJob()
    {
        var services = new ServiceCollection();
        Mock<IDeserializer> deserializer = new();

        services.AddBackgroundJob(jobBuilder =>
        {
            jobBuilder.DisableBackgroundJob = true;
            jobBuilder.UseBackgroundJobCore(_ => { }, _ => deserializer.Object);
        });

        Assert.IsFalse(services.Any(s => s.ServiceType == typeof(IProcessingServer)));
        Assert.IsFalse(services.Any(s => s.ServiceType == typeof(IHostedService) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IBackgroundJobProcessor)));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IProcessor)));
    }

    [TestMethod]
    public void TestUseBackgroundJobByMulti()
    {
        var services = new ServiceCollection();
        Mock<IDeserializer> deserializer = new();
        Mock<IBackgroundJobStorage> backgroundJobStorage = new();

        services.AddSingleton(backgroundJobStorage.Object);
        services.AddBackgroundJob(jobBuilder =>
        {
            jobBuilder.UseBackgroundJobCore(backgroundJobOptions => { backgroundJobOptions.PollInterval = 1; },
                _ => deserializer.Object);
            jobBuilder.UseBackgroundJobCore(backgroundJobOptions => { backgroundJobOptions.PollInterval = 2; },
                _ => deserializer.Object);
        });

        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IBackgroundJobProcessor) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IProcessor) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IProcessingServer) && s.Lifetime == ServiceLifetime.Singleton));
        Assert.IsTrue(services.Any(s => s.ServiceType == typeof(IHostedService) && s.Lifetime == ServiceLifetime.Singleton));

        var serviceProvider = services.BuildServiceProvider();
        var processors = serviceProvider.GetServices<IProcessor>();
        Assert.AreEqual(1, processors.Count());

        Assert.IsNotNull(serviceProvider.GetService<RegisterAccountBackgroundJob>());

        var backgroundJobProcessor = serviceProvider.GetRequiredService<IBackgroundJobProcessor>();
        var jobProcessor = backgroundJobProcessor as BackgroundJobProcessor;
        Assert.IsNotNull(jobProcessor);
        Assert.AreEqual(1, jobProcessor.Period);
    }
}
