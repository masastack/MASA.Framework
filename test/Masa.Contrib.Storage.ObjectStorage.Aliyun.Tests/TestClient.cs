namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestClient
{
    private ALiYunStorageOptions _aLiYunStorageOptions;

    [TestInitialize]
    public void Initialize()
    {
        _aLiYunStorageOptions = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
    }

    [TestMethod]
    public void TestGetTokenAndNullLoggerReturnFalse()
    {
        Mock<IMemoryCache> memoryCache = new();
        var client = new Client(_aLiYunStorageOptions, memoryCache.Object, null);
        Assert.ThrowsException<NotSupportedException>(() => client.GetToken(), "GetToken is not supported, please use GetSecurityToken");
    }

    [TestMethod]
    public void TestGetTokenAndNotNullLoggerReturnFalse()
    {
        Mock<IMemoryCache> memoryCache = new();
        var client = new Client(_aLiYunStorageOptions, memoryCache.Object, NullLogger<Client>.Instance);
        Assert.ThrowsException<NotSupportedException>(() => client.GetToken(), "GetToken is not supported, please use GetSecurityToken");
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheReturnSuccess()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        TemporaryCredentialsResponse temporaryCredentials = new(
            "accessKeyId",
            "secretAccessKey",
            "sessionToken",
            DateTime.UtcNow.AddHours(-1));
        memoryCache.Set(_aLiYunStorageOptions.TemporaryCredentialsCacheKey, temporaryCredentials);
        var client = new Client(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(responseBase == temporaryCredentials);
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundReturnSuccess()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
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
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomNullClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        Assert.ThrowsException<Exception>(() => client.GetSecurityToken(), client.Message);
        Assert.IsNull(memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [TestMethod]
    public void TestSetTemporaryCredentialsAndExpirationLessThan10SecondsReturnSkip()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
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
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        client.TestExpirationTimeLessThan10Second(durationSeconds);
        var res = memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey);
        Assert.IsNotNull(res);
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsReturnNull()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        Assert.ThrowsException<ClientException>(() => client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKey",
            "accessSecret",
            "roleArn",
            "roleSessionName",
            String.Empty,
            3600));
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsAndNullLoggerReturnThrowException()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, null);
        Assert.ThrowsException<ClientException>(() => client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKey",
            "accessSecret",
            "roleArn",
            "roleSessionName",
            "policy",
            3600));
    }
}
