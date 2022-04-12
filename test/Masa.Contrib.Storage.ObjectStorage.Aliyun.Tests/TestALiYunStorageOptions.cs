namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestALiYunStorageOptions
{
    [DataTestMethod]
    [DataRow("", "SecretKey", "RegionId", "RoleArn", "RoleSessionName", "accessKey")]
    [DataRow("AccessKey", "", "RegionId", "RoleArn", "RoleSessionName", "secretKey")]
    [DataRow("AccessKey", "SecretKey", "", "RoleArn", "RoleSessionName", "regionId")]
    [DataRow("AccessKey", "SecretKey", "RegionId", "", "RoleSessionName", "roleArn")]
    [DataRow("AccessKey", "SecretKey", "RegionId", "RoleArn", "", "roleSessionName")]
    public void TestErrorParameterThrowArgumentException(
        string accessKey,
        string secretKey,
        string regionId,
        string roleArn,
        string roleSessionName,
        string parameterName)
    {
        Assert.ThrowsException<ArgumentException>(() =>
                new ALiYunStorageOptions(accessKey, secretKey, regionId, roleArn, roleSessionName),
            $"{parameterName} cannot be empty");
    }

    [TestMethod]
    public void TestDurationSecondsGreaterThan43200ReturnThrowArgumentOutOfRangeException()
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                options.SetDurationSeconds(43201),
            "DurationSeconds must be in range of 900-43200");
    }

    [TestMethod]
    public void TestDurationSecondsLessThan900ReturnThrowArgumentOutOfRangeException()
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                options.SetDurationSeconds(899),
            "DurationSeconds must be in range of 900-43200");
    }

    [DataTestMethod]
    [DataRow("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName")]
    public void TestSuccessParameterReturnInitializationSuccessful(
        string accessKey,
        string secretKey,
        string regionId,
        string roleArn,
        string roleSessionName)
    {
        var options = new ALiYunStorageOptions(accessKey, secretKey, regionId, roleArn, roleSessionName);
        Assert.IsTrue(options.AccessKey == accessKey);
        Assert.IsTrue(options.SecretKey == secretKey);
        Assert.IsTrue(options.RegionId == regionId);
        Assert.IsTrue(options.RoleArn == roleArn);
        Assert.IsTrue(options.RoleSessionName == roleSessionName);
    }

    [DataTestMethod]
    [DataRow(900)]
    [DataRow(901)]
    [DataRow(43200)]
    [DataRow(43199)]
    [DataRow(2000)]
    public void TestDurationSecondsLessThan43200AndGreaterThan900ReturnTrue(int durationSeconds)
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        options.SetDurationSeconds(durationSeconds);
        Assert.IsTrue(options.DurationSeconds == durationSeconds);
    }

    [DataTestMethod]
    [DataRow("", "temporaryCredentialsCacheKey")]
    [DataRow(null, "temporaryCredentialsCacheKey")]
    public void TestErrorTemporaryCredentialsCacheKeyReturnThrowArgumentException(
        string temporaryCredentialsCacheKey,
        string temporaryCredentialsCacheKeyName)
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        Assert.ThrowsException<ArgumentException>(() =>
                options.SetTemporaryCredentialsCacheKey(temporaryCredentialsCacheKey),
            $"{temporaryCredentialsCacheKeyName} cannot be empty");
    }

    [DataTestMethod]
    [DataRow("Aliyun.TemporaryCredentials")]
    public void TestNotNullTemporaryCredentialsCacheKeyReturnSuccess(string temporaryCredentialsCacheKey)
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        options.SetTemporaryCredentialsCacheKey(temporaryCredentialsCacheKey);
        Assert.IsTrue(options.TemporaryCredentialsCacheKey == temporaryCredentialsCacheKey);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("Policy")]
    public void TestSetPolicyReturnSuccess(string policy)
    {
        var options = new ALiYunStorageOptions("AccessKey", "SecretKey", "RegionId", "RoleArn", "RoleSessionName");
        options.SetPolicy(policy);
        Assert.IsTrue(options.Policy == policy);
    }
}
