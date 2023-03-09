// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class StorageTest : TestBase
{
    private static FieldInfo _bucketNameFieldInfo => typeof(DefaultObjectStorageClientContainer).GetField("_bucketName",
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
    public void TestAddAliyunStorageByJsonReturnGetOptions()
    {
        var services = new ServiceCollection();
        var configurationRoot = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        services.AddSingleton<IConfiguration>(configurationRoot);
        services.AddObjectStorage(options => options.UseAliyunStorage());
        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();
        Assert.IsNotNull(optionProvider);
        Assert.IsFalse(optionProvider.SupportCallback);
        Assert.IsFalse(optionProvider.IncompleteStsOptions);

        var aliyunStorageOptions = optionProvider.GetOptions();
        Assert.IsNotNull(aliyunStorageOptions);

        Assert.IsTrue(aliyunStorageOptions.AccessKeyId == "Replace-With-Your-AccessKeyId");
        Assert.IsTrue(aliyunStorageOptions.AccessKeySecret == "Replace-With-Your-AccessKeySecret");
        Assert.IsNotNull(aliyunStorageOptions.Sts);
        Assert.IsTrue(aliyunStorageOptions.Sts.RegionId == "cn-hangzhou");
        Assert.IsTrue(aliyunStorageOptions.Sts.DurationSeconds == 900);
        Assert.IsTrue(aliyunStorageOptions.Endpoint == "oss-cn-hangzhou.aliyuncs.com");
        Assert.IsTrue(aliyunStorageOptions.TemporaryCredentialsCacheKey == "Aliyun.Storage.TemporaryCredentials");
        Assert.IsTrue(aliyunStorageOptions.Policy == "Policy");
        Assert.IsTrue(aliyunStorageOptions.RoleArn == "Replace-With-Your-RoleArn");
        Assert.IsTrue(aliyunStorageOptions.RoleSessionName == "Replace-With-Your-RoleSessionName");
    }

    [TestMethod]
    public void TestAddAliyunStorageByOptions()
    {
        var services = new ServiceCollection();
        services.AddObjectStorage(options => options.UseAliyunStorage(_aLiYunStorageOptions));

        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();

        Assert.IsNotNull(optionProvider);
        Assert.IsFalse(optionProvider.SupportCallback);
        Assert.IsTrue(optionProvider.IncompleteStsOptions);

        var options = optionProvider.GetOptions();
        Assert.IsTrue(options.AccessKeyId == _aLiYunStorageOptions.AccessKeyId);
        Assert.IsTrue(options.AccessKeySecret == _aLiYunStorageOptions.AccessKeySecret);
        Assert.IsTrue(options.Endpoint == _aLiYunStorageOptions.Endpoint);
        Assert.IsTrue(options.RoleArn == _aLiYunStorageOptions.RoleArn);
        Assert.IsTrue(options.RoleSessionName == _aLiYunStorageOptions.RoleSessionName);
    }

    [TestMethod]
    public void TestAddAliyunStorage()
    {
        var services = new ServiceCollection();
        services.Configure<AliyunStorageConfigureOptions>(option =>
        {
            option.AccessKeyId = "AccessKeyId";
            option.AccessKeySecret = "AccessKeySecret";
            option.Sts = new AliyunStsOptions("RegionId")
            {
                DurationSeconds = 900,
                EarlyExpires = 10,
            };
            option.Storage = new AliyunStorageOptions()
            {
                Endpoint = "Endpoint",
                TemporaryCredentialsCacheKey = "TemporaryCredentialsCacheKey",
                Policy = "Policy",
                RoleArn = "RoleArn",
                RoleSessionName = "RoleSessionName",
                CallbackUrl = "CallbackUrl",
                CallbackBody = "CallbackBody",
                EnableResumableUpload = true,
                BigObjectContentLength = 200,
                PartSize = 10,
                Quiet = true
            };
        });
        services.AddObjectStorage(options => options.UseAliyunStorage());

        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();

        Assert.IsNotNull(optionProvider);
        Assert.IsTrue(optionProvider.SupportCallback);
        Assert.IsFalse(optionProvider.IncompleteStsOptions);

        var options = optionProvider.GetOptions();

        Assert.IsNotNull(options.Sts);
        Assert.IsTrue(options.Sts.DurationSeconds == 900);
        Assert.IsTrue(options.Sts.EarlyExpires == 10);
        Assert.IsTrue(options.Sts.RegionId == "RegionId");
        Assert.IsTrue(options.AccessKeyId == "AccessKeyId");
        Assert.IsTrue(options.AccessKeySecret == "AccessKeySecret");
        Assert.IsTrue(options.Endpoint == "Endpoint");
        Assert.IsTrue(options.RoleArn == "RoleArn");
        Assert.IsTrue(options.RoleSessionName == "RoleSessionName");
        Assert.IsTrue(options.TemporaryCredentialsCacheKey == "TemporaryCredentialsCacheKey");
        Assert.IsTrue(options.Policy == "Policy");
        Assert.IsTrue(options.CallbackUrl == "CallbackUrl");
        Assert.IsTrue(options.CallbackBody == "CallbackBody");
        Assert.IsTrue(options.EnableResumableUpload == true);
        Assert.IsTrue(options.BigObjectContentLength == 200);
        Assert.IsTrue(options.PartSize == 10);
        Assert.IsTrue(options.Quiet);
    }

    [TestMethod]
    public void TestAddAliyunStorage2()
    {
        var services = new ServiceCollection();
        services.Configure<AliyunStorageConfigureOptions>(option =>
        {
            option.AccessKeyId = "AccessKeyId";
            option.AccessKeySecret = "AccessKeySecret";
            option.Sts = new AliyunStsOptions("RegionId")
            {
                DurationSeconds = 900,
                EarlyExpires = 10,
            };
            option.Storage = new AliyunStorageOptions()
            {
                Sts = new AliyunStsOptions()
                {
                    DurationSeconds = 1200,
                    EarlyExpires = 100,
                    RegionId = "RegionId2"
                },
                AccessKeyId = "AccessKeyId2",
                AccessKeySecret = "AccessKeySecret2",
                Endpoint = "Endpoint",
                TemporaryCredentialsCacheKey = "TemporaryCredentialsCacheKey",
                Policy = "Policy",
                RoleArn = "RoleArn",
                RoleSessionName = "RoleSessionName",
                CallbackUrl = "CallbackUrl",
                CallbackBody = "CallbackBody",
                EnableResumableUpload = true,
                BigObjectContentLength = 200,
                PartSize = 10,
                Quiet = true
            };
        });
        services.AddObjectStorage(options => options.UseAliyunStorage());

        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();

        Assert.IsNotNull(optionProvider);
        Assert.IsTrue(optionProvider.SupportCallback);
        Assert.IsFalse(optionProvider.IncompleteStsOptions);

        var options = optionProvider.GetOptions();

        Assert.IsNotNull(options.Sts);

        Assert.IsTrue(options.Sts.DurationSeconds == 1200);
        Assert.IsTrue(options.Sts.EarlyExpires == 100);
        Assert.IsTrue(options.Sts.RegionId == "RegionId2");
        Assert.IsTrue(options.AccessKeyId == "AccessKeyId2");
        Assert.IsTrue(options.AccessKeySecret == "AccessKeySecret2");
        Assert.IsTrue(options.Endpoint == "Endpoint");
        Assert.IsTrue(options.RoleArn == "RoleArn");
        Assert.IsTrue(options.RoleSessionName == "RoleSessionName");
        Assert.IsTrue(options.TemporaryCredentialsCacheKey == "TemporaryCredentialsCacheKey");
        Assert.IsTrue(options.Policy == "Policy");
        Assert.IsTrue(options.CallbackUrl == "CallbackUrl");
        Assert.IsTrue(options.CallbackBody == "CallbackBody");
        Assert.IsTrue(options.EnableResumableUpload == true);
        Assert.IsTrue(options.BigObjectContentLength == 200);
        Assert.IsTrue(options.PartSize == 10);
        Assert.IsTrue(options.Quiet);
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
        services.AddObjectStorage(options => options.UseAliyunStorage(_aLiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();
        var bucketNameProvider = serviceProvider.GetService<IBucketNameProvider>();
        Assert.IsNotNull(bucketNameProvider);
        Assert.AreEqual(_aLiYunStorageOptions.BucketNames.DefaultBucketName, bucketNameProvider.GetBucketName());
    }

    [TestMethod]
    public void TestClientContainer()
    {
        var services = new ServiceCollection();
        _aLiYunStorageOptions.BucketNames = new BucketNames(new List<KeyValuePair<string, string>>()
        {
            new("test-bucket", "test")
        });
        services.AddObjectStorage(options => options.UseAliyunStorage(_aLiYunStorageOptions));
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
        services.AddObjectStorage(options => options.UseAliyunStorage(_aLiYunStorageOptions));
        var serviceProvider = services.BuildServiceProvider();

        var clientContainerFactory = serviceProvider.GetService<IObjectStorageClientContainerFactory>();
        Assert.IsNotNull(clientContainerFactory);

        var clientContainer = clientContainerFactory.Create();
        var bucketName = GetBucketName(clientContainer);
        Assert.IsNotNull(bucketName);
        Assert.AreEqual(BucketNames.DEFAULT_BUCKET_NAME, bucketName);
    }

    private static string? GetBucketName(IObjectStorageClientContainer objectStorageClientContainer)
        => _bucketNameFieldInfo.GetValue(objectStorageClientContainer)!.ToString();
}
