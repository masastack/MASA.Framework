// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestStorage : BaseTest
{
    private const string HANG_ZHOUE_REGIONID = "oss-cn-hangzhou";

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
        var client = serviceProvider.GetService<IClient>();
        Assert.IsNotNull(client);
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
        services.AddAliyunStorage();
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
        services.AddAliyunStorage(_aLiYunStorageOptions);

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
        services.AddAliyunStorage();

        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();

        Assert.IsNotNull(optionProvider);
        Assert.IsTrue(optionProvider.SupportCallback);
        Assert.IsFalse(optionProvider.IncompleteStsOptions);

        var options = optionProvider.GetOptions();
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
        services.AddAliyunStorage();

        var serviceProvider = services.BuildServiceProvider();
        var optionProvider = serviceProvider.GetService<IAliyunStorageOptionProvider>();

        Assert.IsNotNull(optionProvider);
        Assert.IsTrue(optionProvider.SupportCallback);
        Assert.IsFalse(optionProvider.IncompleteStsOptions);

        var options = optionProvider.GetOptions();
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
        services.AddAliyunStorage(new AliyunStorageOptions("accessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "roleArn",
            "roleSessionName"));
        var serviceProvider = services.BuildServiceProvider();
        Assert.IsNotNull(serviceProvider.GetService<IClient>());
    }

    [TestMethod]
    public void TestAddMultiAliyunStorageReturnClientCountIs1()
    {
        var services = new ServiceCollection();
        AliyunStorageOptions options =
            new AliyunStorageOptions("accessKeyId", "accessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "roleArn", "roleSessionName");
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
