// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests.Isolation;

[TestClass]
public class StorageTest
{
    private readonly IServiceCollection _services;

    public StorageTest()
    {
        _services = new ServiceCollection();
    }

    [TestMethod]
    public void TestAddStorage()
    {
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        _services.AddSingleton<IConfiguration>(configurationRoot);
        _services.AddObjectStorage(objectStorageBuilder =>
        {
            objectStorageBuilder.UseAliyunStorage();
        });
        _services.AddIsolation(isolationBuilder =>
        {
            isolationBuilder.UseMultiTenant();
        });
        var rootServiceProvider = _services.BuildServiceProvider();
        var objectStorage = GetObjectStorage();

        var aliyunStorageOptions = ObjectStorageUtils.GetAliyunStorageOptions(objectStorage.ObjectStorageClient);

        Assert.AreEqual("test-storage", aliyunStorageOptions.BucketNames.DefaultBucketName);
        Assert.AreEqual("test-storage2", aliyunStorageOptions.BucketNames.GetBucketName("test-bucket"));

        Assert.AreEqual("test-storage", objectStorage.BucketNameProvider.GetBucketName());
        Assert.AreEqual("test-storage2", objectStorage.BucketNameProvider.GetBucketName("test-bucket"));

        objectStorage = GetObjectStorage(serviceProvider =>
        {
            var multiTenantSetter = serviceProvider.GetService<IMultiTenantSetter>();
            Assert.IsNotNull(multiTenantSetter);
            multiTenantSetter.SetTenant(new Tenant("1"));
        });
        aliyunStorageOptions = ObjectStorageUtils.GetAliyunStorageOptions(objectStorage.ObjectStorageClient);

        Assert.AreEqual("isolation-test-storage", aliyunStorageOptions.BucketNames.DefaultBucketName);
        Assert.AreEqual("isolation-test-storage2", aliyunStorageOptions.BucketNames.GetBucketName("test-bucket"));

        Assert.AreEqual("isolation-test-storage", objectStorage.BucketNameProvider.GetBucketName());
        Assert.AreEqual("isolation-test-storage2", objectStorage.BucketNameProvider.GetBucketName("test-bucket"));

        (IObjectStorageClient ObjectStorageClient,
            IBucketNameProvider BucketNameProvider,
            IObjectStorageClientContainer ObjectStorageClientContainer) GetObjectStorage(Action<IServiceProvider>? action = null)
        {
            using var scope = rootServiceProvider.CreateScope();
            action?.Invoke(scope.ServiceProvider);
            var objectStorageClient = scope.ServiceProvider.GetService<IObjectStorageClient>();
            var bucketNameProvider = scope.ServiceProvider.GetService<IBucketNameProvider>();
            var objectStorageClientContainer = scope.ServiceProvider.GetService<IObjectStorageClientContainer>();
            Assert.IsNotNull(objectStorageClient);
            Assert.IsNotNull(bucketNameProvider);
            Assert.IsNotNull(objectStorageClientContainer);
            return (objectStorageClient, bucketNameProvider, objectStorageClientContainer);
        }
    }

    [TestMethod]
    public void TestAddStorage2()
    {
        TestAddStorage();

        _services.AddObjectStorage("aliyun2", objectStorageBuilder =>
        {
            objectStorageBuilder.UseAliyunStorage("Aliyun2");
        });
        var rootServiceProvider = _services.BuildServiceProvider();
        Assert.IsNotNull(rootServiceProvider);

        var objectStorage = GetObjectStorage();

        var aliyunStorageOptions = ObjectStorageUtils.GetAliyunStorageOptions(objectStorage.ObjectStorageClient);

        Assert.AreEqual("test-storage-aliyun2", aliyunStorageOptions.BucketNames.DefaultBucketName);
        Assert.AreEqual("test-storage2-aliyun2", aliyunStorageOptions.BucketNames.GetBucketName("test-bucket"));

        Assert.AreEqual("test-storage-aliyun2", objectStorage.BucketNameProvider.GetBucketName());
        Assert.AreEqual("test-storage2-aliyun2", objectStorage.BucketNameProvider.GetBucketName("test-bucket"));

        objectStorage = GetObjectStorage(serviceProvider =>
        {
            var multiTenantSetter = serviceProvider.GetService<IMultiTenantSetter>();
            Assert.IsNotNull(multiTenantSetter);
            multiTenantSetter.SetTenant(new Tenant("1"));
        });

        aliyunStorageOptions = ObjectStorageUtils.GetAliyunStorageOptions(objectStorage.ObjectStorageClient);

        Assert.AreEqual("isolation-test-storage-aliyun2", aliyunStorageOptions.BucketNames.DefaultBucketName);
        Assert.AreEqual("isolation-test-storage2-aliyun2", aliyunStorageOptions.BucketNames.GetBucketName("test-bucket"));

        Assert.AreEqual("isolation-test-storage-aliyun2", objectStorage.BucketNameProvider.GetBucketName());
        Assert.AreEqual("isolation-test-storage2-aliyun2", objectStorage.BucketNameProvider.GetBucketName("test-bucket"));

        (IObjectStorageClient ObjectStorageClient,
            IBucketNameProvider BucketNameProvider,
            IObjectStorageClientContainer ObjectStorageClientContainer) GetObjectStorage(Action<IServiceProvider>? action = null)
        {
            using var scope = rootServiceProvider.CreateScope();
            var objectStorageClientFactory = scope.ServiceProvider.GetRequiredService<IObjectStorageClientFactory>();
            var bucketNameFactory = scope.ServiceProvider.GetRequiredService<IBucketNameFactory>();
            var objectStorageClientContainerFactory = scope.ServiceProvider.GetRequiredService<IObjectStorageClientContainerFactory>();

            action?.Invoke(scope.ServiceProvider);
            var objectStorageClient = objectStorageClientFactory.Create("aliyun2");
            var bucketNameProvider = bucketNameFactory.Create("aliyun2");
            var objectStorageClientContainer = objectStorageClientContainerFactory.Create("aliyun2");
            Assert.IsNotNull(objectStorageClient);
            Assert.IsNotNull(bucketNameProvider);
            Assert.IsNotNull(objectStorageClientContainer);
            return (objectStorageClient, bucketNameProvider, objectStorageClientContainer);
        }
    }
}
