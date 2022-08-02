// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestCredentialProvider : BaseTest
{
    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundReturnSuccess()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        var client = new CustomCredentialProvider(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        var securityToken = client.GetSecurityToken();
        Assert.IsTrue(securityToken.Expiration == client.TemporaryCredentials.Expiration &&
            securityToken.AccessKeyId == client.TemporaryCredentials.AccessKeyId &&
            securityToken.AccessKeySecret == client.TemporaryCredentials.AccessKeySecret &&
            securityToken.SessionToken == client.TemporaryCredentials.SessionToken);
        Assert.IsNotNull(memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundAndGetTemporaryCredentialsIsNullReturnError()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomNullClient(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        Assert.ThrowsException<Exception>(() => client.GetSecurityToken(), client.Message);
        Assert.IsNull(memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [TestMethod]
    public void TestSetTemporaryCredentialsAndExpirationLessThan10SecondsReturnSkip()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomCredentialProvider(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        client.TestExpirationTimeLessThan10Second(5);
        Assert.IsNull(memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [DataTestMethod]
    [DataRow(15)]
    [DataRow(20)]
    public void TestSetTemporaryCredentialsAndExpirationGreatherThanOrEqual10SecondsReturnSkip(int durationSeconds)
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomCredentialProvider(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        client.TestExpirationTimeLessThan10Second(durationSeconds);
        var res = memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey);
        Assert.IsNotNull(res);
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsReturnNull()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomCredentialProvider(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        Assert.ThrowsException<ClientException>(() => client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKeyId",
            "accessKeySecret",
            "roleArn",
            "roleSessionName",
            string.Empty,
            3600));
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsAndNullLoggerReturnThrowException()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomCredentialProvider(serviceProvider.GetRequiredService<IOssClientFactory>(),
            MockOptionProvider().Object,
            memoryCache,
            NullLogger<DefaultCredentialProvider>.Instance);
        Assert.ThrowsException<ClientException>(() => client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKeyId",
            "accessKeySecret",
            "roleArn",
            "roleSessionName",
            "policy",
            3600));
    }
}
