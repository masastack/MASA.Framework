// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestStorage
{
    private const string HANG_ZHOUE_REGIONID = "oss-cn-hangzhou";

    private const string HANG_ZHOUE_PUBLIC_ENDPOINT = "oss-cn-hangzhou.aliyuncs.com";

    [TestMethod]
    public void TestAddAliyunStorageAndNotAddConfigurationReturnClientIsNotNull()
    {
        IServiceCollection services = new ServiceCollection();
        services.AddAliyunStorage();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
    }

    [TestMethod]
    public void TestAddAliyunStorageByEmptySectionReturnThrowArgumentException()
    {
        var services = new ServiceCollection();
        Assert.ThrowsException<ArgumentException>(() => services.AddAliyunStorage(string.Empty));
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
        services.AddAliyunStorage();
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
    }

    [TestMethod]
    public void TestAddAliyunStorageAndNullALiYunStorageOptionsReturnThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        AliyunStorageOptions aLiYunStorageOptions = null!;
        Assert.ThrowsException<ArgumentNullException>(() => services.AddAliyunStorage(aLiYunStorageOptions));
    }

    [TestMethod]
    public void TestAddAliyunStorageByEmptyAccessKeyIdReturnThrowArgumentNullException()
    {
        Assert.ThrowsException<ArgumentException>(() => new AliyunStorageOptions(null!, null!, null!));
    }

    [TestMethod]
    public void TestAddAliyunStorageByALiYunStorageOptionsReturnClientNotNull()
    {
        var services = new ServiceCollection();
        services.AddAliyunStorage(new AliyunStorageOptions("accessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "roleArn", "roleSessionName"));
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
    }

    [TestMethod]
    public void TestAddAliyunStorageByAccessKeyIdAndAccessKeySecretAndRegionIdReturnClientNotNull()
    {
        var services = new ServiceCollection();
        services.AddAliyunStorage("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_REGIONID, Options.Enum.EndpointMode.Public);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
    }

    [TestMethod]
    public void TestAddMultiAliyunStorageReturnClientCountIs1()
    {
        var services = new ServiceCollection();
        AliyunStorageOptions options = new AliyunStorageOptions("accessKeyId", "accessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "roleArn", "roleSessionName");
        services.AddAliyunStorage(options).AddAliyunStorage(options);
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
        Assert.IsTrue(serviceProvider.GetServices<IClient>().Count() == 1);
    }

    [TestMethod]
    public void TestAddAliyunStorageAndNullALiYunStorageOptionsFuncReturnThrowArgumentNullException()
    {
        var services = new ServiceCollection();
        Func<AliyunStorageOptions> func = null!;
        Assert.ThrowsException<ArgumentNullException>(() => services.AddAliyunStorage(func));
    }
}
