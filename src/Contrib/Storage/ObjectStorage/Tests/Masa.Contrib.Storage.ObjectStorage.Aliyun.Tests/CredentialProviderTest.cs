// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class CredentialProviderTest : TestBase
{
    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundReturnSuccess()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

        var client = new CustomCredentialProvider(ALiYunStorageOptions, memoryCache);
        var securityToken = client.GetSecurityToken();
        Assert.IsTrue(securityToken.Expiration == client.TemporaryCredentials.Expiration &&
            securityToken.AccessKeyId == client.TemporaryCredentials.AccessKeyId &&
            securityToken.AccessKeySecret == client.TemporaryCredentials.AccessKeySecret &&
            securityToken.SessionToken == client.TemporaryCredentials.SessionToken);
        Assert.IsNotNull(memoryCache.Get<TemporaryCredentialsResponse>(ALiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundAndGetTemporaryCredentialsIsNullReturnError()
    {
        var client = new CustomExceptionCredentialProvider(ALiYunStorageOptions, new MemoryCache(new MemoryDistributedCacheOptions()));
        Assert.ThrowsExactly<Exception>(() => client.GetSecurityToken(), client.Message);
    }

    [DataTestMethod]
    [DataRow(5, true)]
    [DataRow(15, false)]
    [DataRow(20, false)]
    public void TestSetTemporaryCredentials(int durationSeconds, bool isNull)
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        services.AddSingleton<IOssClientFactory, DefaultOssClientFactory>();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomCredentialProvider(
            ALiYunStorageOptions,
            memoryCache,
            serviceProvider.GetRequiredService<IOssClientFactory>(),
            NullLoggerFactory.Instance);
        client.TestExpirationTimeLessThan(durationSeconds);

        var result = memoryCache.Get<TemporaryCredentialsResponse>(ALiYunStorageOptions.TemporaryCredentialsCacheKey);
        if (isNull)
        {
            Assert.IsNull(result);
        }
        else
        {
            Assert.IsNotNull(result);
        }
    }
}
