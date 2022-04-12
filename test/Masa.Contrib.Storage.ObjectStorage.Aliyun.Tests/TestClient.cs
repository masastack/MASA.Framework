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
        var responseBase = client.GetToken();
        Assert.IsTrue(!responseBase.IsValid && responseBase.Message == "GetToken is not supported, please use GetSecurityToken");
    }

    [TestMethod]
    public void TestGetTokenAndNotNullLoggerReturnFalse()
    {
        Mock<IMemoryCache> memoryCache = new();
        var client = new Client(_aLiYunStorageOptions, memoryCache.Object, NullLogger<Client>.Instance);
        var responseBase = client.GetToken();
        Assert.IsTrue(!responseBase.IsValid && responseBase.Message == "GetToken is not supported, please use GetSecurityToken");
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
            DateTime.UtcNow.AddHours(-1).ToString(CultureInfo.InvariantCulture));
        memoryCache.Set(_aLiYunStorageOptions.TemporaryCredentialsCacheKey, temporaryCredentials);
        var client = new Client(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(responseBase.IsValid && responseBase.Message == "success" && responseBase.Data == temporaryCredentials);
    }

    [TestMethod]
    public void TestGetSecurityTokenByCacheNotFoundReturnSuccess()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(responseBase.IsValid && responseBase.Message == "success" &&
            System.Text.Json.JsonSerializer.Serialize(responseBase.Data) ==
            System.Text.Json.JsonSerializer.Serialize(client.TemporaryCredentials));
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
        var responseBase = client.GetSecurityToken();
        Assert.IsTrue(!responseBase.IsValid && responseBase.Message == client.Message && responseBase.Data == null);
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
    [DataRow(10)]
    [DataRow(20)]
    public void TestSetTemporaryCredentialsAndExpirationGreatherThanOrEqual10SecondsReturnSkip(int durationSeconds)
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        client.TestExpirationTimeLessThan10Second(durationSeconds);
        Assert.IsNotNull(memoryCache.Get<TemporaryCredentialsResponse>(_aLiYunStorageOptions.TemporaryCredentialsCacheKey));
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsReturnNull()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, NullLogger<Client>.Instance);
        string message = String.Empty;
        var temporaryCredentials = client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKey",
            "accessSecret",
            "roleArn",
            "roleSessionName",
            String.Empty,
            3600,
            error => message = error);
        Assert.IsNull(temporaryCredentials);
        Assert.IsTrue(!string.IsNullOrEmpty(message));
    }

    [TestMethod]
    public void TestGetTemporaryCredentialsAndNullLOggerReturnNull()
    {
        var services = new ServiceCollection();
        services.AddMemoryCache();
        var serviceProvider = services.BuildServiceProvider();
        var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
        var client = new CustomClient(_aLiYunStorageOptions, memoryCache, null);
        string message = String.Empty;
        var temporaryCredentials = client.TestGetTemporaryCredentials(
            "cn-shanghai",
            "accessKey",
            "accessSecret",
            "roleArn",
            "roleSessionName",
            "policy",
            3600,
            error => message = error);
        Assert.IsNull(temporaryCredentials);
        Assert.IsTrue(!string.IsNullOrEmpty(message));
    }
}
