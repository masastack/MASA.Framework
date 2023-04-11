// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class StorageTest : TestBase
{
    private static FieldInfo ObjectStorageClientBaseFieldInfo
        => typeof(DefaultStorageClient).GetField("AliyunStorageOptions", BindingFlags.Instance | BindingFlags.NonPublic)!;

    private static FieldInfo BucketNameFieldInfo => typeof(DefaultObjectStorageClientContainer).GetField("_bucketName",
        BindingFlags.Instance | BindingFlags.NonPublic)!;

    [TestMethod]
    public void TestAddAliyunStorageAndNotAddConfiguration()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage());
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IObjectStorageClient>());
        Assert.IsNotNull(serviceProvider.GetService<IObjectStorageClientContainer>());
        Assert.IsNotNull(serviceProvider.GetService<IBucketNameProvider>());
    }

    [TestMethod]
    public void TestAddAliyunStorageReturnClientIsNotNull()
    {
        var services = new ServiceCollection();
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        services.AddSingleton<IConfiguration>(configurationRoot);
        services.AddObjectStorage(options => options.UseAliyunStorage());
        var serviceProvider = services.BuildServiceProvider();
        var client = serviceProvider.GetService<IObjectStorageClient>();
        Assert.IsNotNull(client);

        var bucketProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketProvider);

        Assert.IsTrue(bucketProvider.GetBucketName() == "test-storage");
        Assert.IsTrue(bucketProvider.GetBucketName<StorageContainer>() == "test-storage2");
    }

    [TestMethod]
    public void TestAddAliyunStorageByOptions()
    {
        var services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage(ALiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();

        var objectStorageClient = serviceProvider.GetRequiredService<IManualObjectStorageClient>();
        var aliyunStorageOptions = GetAliyunStorageOptions(GetManualObjectStorageClient(objectStorageClient) as DefaultStorageClient);
        Assert.IsTrue(aliyunStorageOptions.AccessKeyId == ALiYunStorageOptions.AccessKeyId);
        Assert.IsTrue(aliyunStorageOptions.AccessKeySecret == ALiYunStorageOptions.AccessKeySecret);
        Assert.IsTrue(aliyunStorageOptions.Endpoint == ALiYunStorageOptions.Endpoint);
        Assert.IsTrue(aliyunStorageOptions.RoleArn == ALiYunStorageOptions.RoleArn);
        Assert.IsTrue(aliyunStorageOptions.RoleSessionName == ALiYunStorageOptions.RoleSessionName);
    }

    [TestMethod]
    public void TestAddAliyunStorage()
    {
        var services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage());
        var serviceProvider = services.BuildServiceProvider();

        var objectStorageClient = serviceProvider.GetService<IObjectStorageClient>();
        Assert.IsNotNull(objectStorageClient);

        var bucketNameProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketNameProvider);
        var bucketName = bucketNameProvider.GetBucketName();
        Assert.AreEqual(BucketNames.DEFAULT_BUCKET_NAME, bucketName);
    }

    [TestMethod]
    public void TestAddAliyunStorageAndUseAppsettings()
    {
        var services = new ServiceCollection();
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        services.AddSingleton<IConfiguration>(configurationRoot);
        services.AddObjectStorage(options => options.UseAliyunStorage());
        var serviceProvider = services.BuildServiceProvider();

        var objectStorageClient = serviceProvider.GetService<IObjectStorageClient>();
        Assert.IsNotNull(objectStorageClient);

        var bucketNameProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketNameProvider);
        var bucketName = bucketNameProvider.GetBucketName();
        Assert.AreEqual("test-storage", bucketName);
        Assert.AreEqual("test-storage2", bucketNameProvider.GetBucketName("test-bucket"));
    }

    [TestMethod]
    public void TestAddAliyunStorageAndNullALiYunStorageOptionsReturnThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        AliyunStorageOptions aLiYunStorageOptions = null!;
        Assert.ThrowsException<MasaArgumentException>(()
            => services.AddObjectStorage(options => options.UseAliyunStorage(aLiYunStorageOptions)));
    }

    [TestMethod]
    public void TestAddAliyunStorageByEmptyAccessKeyIdReturnThrowArgumentNullException()
    {
        Assert.ThrowsException<MasaArgumentException>(() => new AliyunStorageOptions(null!, null!, null!));
    }

    [TestMethod]
    public void TestAddAliyunStorageByALiYunStorageOptionsReturnClientNotNull()
    {
        var services = new ServiceCollection();
        var aliyunStorageOptions = new AliyunStorageOptions("accessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "roleArn",
            "roleSessionName");
        services.AddObjectStorage(options => options.UseAliyunStorage(aliyunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IObjectStorageClient>());
    }

    [TestMethod]
    public void TestAddAliyunStorageAndNullALiYunStorageOptionsFuncReturnThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Func<AliyunStorageOptions> func = null!;
        Assert.ThrowsException<MasaArgumentException>(() => services.AddObjectStorage(options => options.UseAliyunStorage(func)));
    }

    [TestMethod]
    public void TestDefaultBucketNameReturnDefaultBucketNameEqualTest()
    {
        var services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage(ALiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();
        var bucketNameProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketNameProvider);
        Assert.AreEqual(ALiYunStorageOptions.BucketNames.DefaultBucketName, bucketNameProvider.GetBucketName());
    }

    [TestMethod]
    public void TestClientContainer()
    {
        var services = new ServiceCollection();
        ALiYunStorageOptions.BucketNames = new BucketNames(new List<KeyValuePair<string, string>>()
        {
            new("test-bucket", "test")
        });
        services.AddObjectStorage(options => options.UseAliyunStorage(ALiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();
        var defaultClientContainer = serviceProvider.GetService<IObjectStorageClientContainer>();
        Assert.IsNotNull(defaultClientContainer);

        var bucketNameProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketNameProvider);
        Assert.IsTrue(bucketNameProvider.GetBucketName<StorageContainer>() == "test");

        var storageContainer = serviceProvider.GetService<IObjectStorageClientContainer<StorageContainer>>();
        Assert.IsNotNull(storageContainer);

        var bucketName = GetBucketName(storageContainer);
        Assert.IsNotNull(bucketName);
        Assert.IsTrue(bucketName == "test");
    }

    [TestMethod]
    public void TestClientFactory()
    {
        var services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage(ALiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();

        var clientContainerFactory = serviceProvider.GetService<IObjectStorageClientContainerFactory>();
        Assert.IsNotNull(clientContainerFactory);

        var clientContainer = clientContainerFactory.Create();
        var bucketName = GetBucketName(clientContainer);
        Assert.IsNotNull(bucketName);
        Assert.AreEqual(BucketNames.DEFAULT_BUCKET_NAME, bucketName);
    }

    private static AliyunStorageOptions GetAliyunStorageOptions(DefaultStorageClient? storageClient)
    {
        Assert.IsNotNull(storageClient);
        var aliyunStorageOptions = ObjectStorageClientBaseFieldInfo.GetValue(storageClient) as AliyunStorageOptions;
        Assert.IsNotNull(aliyunStorageOptions);
        return aliyunStorageOptions;
    }

    private static string? GetBucketName(IObjectStorageClientContainer objectStorageClientContainer)
        => BucketNameFieldInfo.GetValue(objectStorageClientContainer)!.ToString();

    private static IManualObjectStorageClient GetManualObjectStorageClient(IManualObjectStorageClient manualObjectStorageClient)
    {
        return (IManualObjectStorageClient)typeof(DefaultObjectStorageClient)
            .GetField("_objectStorageClient")!
            .GetValue(
            manualObjectStorageClient)!;
    }
}
