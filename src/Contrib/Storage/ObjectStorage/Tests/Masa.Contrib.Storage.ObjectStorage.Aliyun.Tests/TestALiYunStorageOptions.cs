// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

[TestClass]
public class TestALiYunStorageOptions
{
    private const string HANG_ZHOUE_PUBLIC_ENDPOINT = "oss-cn-hangzhou.aliyuncs.com";
    private const string HANG_ZHOUE_INTERNAL_ENDPOINT = "oss-cn-hangzhou-internal.aliyuncs.com";
    private const string TEMPORARY_CREDENTIALS_CACHEKEY = "Aliyun.Storage.TemporaryCredentials";

    [DataTestMethod]
    [DataRow("", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName", "accessKeyId")]
    [DataRow("AccessKeyId", "", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName", "accessKeySecret")]
    [DataRow("AccessKeyId", "AccessKeySecret", "", "RoleArn", "RoleSessionName", "regionId")]
    [DataRow("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "", "RoleSessionName", "roleArn")]
    [DataRow("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "", "roleSessionName")]
    public void TestErrorParameterThrowArgumentException(
        string accessKeyId,
        string accessKeySecret,
        string regionId,
        string roleArn,
        string roleSessionName,
        string parameterName)
    {
        Assert.ThrowsException<MasaArgumentException>(() =>
                new AliyunStorageOptions(accessKeyId, accessKeySecret, regionId, roleArn, roleSessionName),
            $"{parameterName} cannot be empty");
    }

    [TestMethod]
    public void TestDurationSecondsGreaterThan43200ReturnThrowArgumentOutOfRangeException()
    {
        var aliyunStsOptions = new AliyunStsOptions();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                aliyunStsOptions.DurationSeconds = 43201,
            "DurationSeconds must be in range of 900-43200");
    }

    [TestMethod]
    public void TestDurationSecondsLessThan900ReturnThrowArgumentOutOfRangeException()
    {
        var aliyunStsOptions = new AliyunStsOptions();
        Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                aliyunStsOptions.DurationSeconds = 899,
            "DurationSeconds must be in range of 900-43200");
    }

    [DataTestMethod]
    [DataRow("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName")]
    [DataRow("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_INTERNAL_ENDPOINT, "RoleArn", "RoleSessionName")]
    public void TestSuccessParameterReturnInitializationSuccessful(
        string accessKeyId,
        string accessKeySecret,
        string endpoint,
        string roleArn,
        string roleSessionName)
    {
        var options = new AliyunStorageOptions(accessKeyId, accessKeySecret, endpoint, roleArn, roleSessionName);
        Assert.IsTrue(options.AccessKeyId == accessKeyId);
        Assert.IsTrue(options.AccessKeySecret == accessKeySecret);
        Assert.IsTrue(options.Sts.RegionId == null);
        Assert.IsTrue(options.RoleArn == roleArn);
        Assert.IsTrue(options.RoleSessionName == roleSessionName);
    }

    [DataTestMethod]
    [DataRow("", "temporaryCredentialsCacheKey")]
    [DataRow(null, "temporaryCredentialsCacheKey")]
    public void TestErrorTemporaryCredentialsCacheKeyReturnThrowArgumentException(
        string temporaryCredentialsCacheKey,
        string temporaryCredentialsCacheKeyName)
    {
        var options = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName");
        Assert.ThrowsException<MasaArgumentException>(() =>
                options.SetTemporaryCredentialsCacheKey(temporaryCredentialsCacheKey),
            $"{temporaryCredentialsCacheKeyName} cannot be empty");
    }

    [DataTestMethod]
    [DataRow("Aliyun.TemporaryCredentials")]
    public void TestNotNullTemporaryCredentialsCacheKeyReturnSuccess(string temporaryCredentialsCacheKey)
    {
        var options = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName");
        options.SetTemporaryCredentialsCacheKey(temporaryCredentialsCacheKey);
        Assert.IsTrue(options.TemporaryCredentialsCacheKey == temporaryCredentialsCacheKey);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(null)]
    [DataRow("Policy")]
    public void TestSetPolicyReturnSuccess(string policy)
    {
        var options = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT, "RoleArn", "RoleSessionName");
        options.SetPolicy(policy);
        Assert.IsTrue(options.Policy == policy);
    }

    [DataTestMethod]
    [DataRow("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT)]
    public void TestNullRoleArnAndSessionNameReturnSessionNameIsNull(
        string accessKeyId,
        string accessKeySecret,
        string endpoint)
    {
        var options = new AliyunStorageOptions(accessKeyId, accessKeySecret, endpoint);
        Assert.IsTrue(options.Endpoint == HANG_ZHOUE_PUBLIC_ENDPOINT);
        Assert.IsTrue(options.TemporaryCredentialsCacheKey == TEMPORARY_CREDENTIALS_CACHEKEY);
        Assert.IsTrue(options.Quiet);
        Assert.IsNotNull(options.CallbackBody);
        Assert.IsTrue(options.EnableResumableUpload);
        Assert.IsTrue(options.BigObjectContentLength == 5 * 1024L * 1024 * 1024);
        Assert.IsNull(options.RoleArn);
        Assert.IsNull(options.RoleSessionName);


    }

    [TestMethod]
    public void TestAliyunStsOptionsDefaultReturnDurationSecondsIs3600()
    {
        AliyunStsOptions stsOptions = new AliyunStsOptions();
        Assert.IsTrue(stsOptions.RegionId == null);
        Assert.IsTrue(stsOptions.GetEarlyExpires() == 10);
        Assert.IsTrue(stsOptions.GetDurationSeconds() == 3600);
    }

    [TestMethod]
    public void TestEarlyExpireLessThanZeroReturnThrowArgumentOutOfRangeException()
    {
        var options = new AliyunStorageOptions("AccessKeyId", "AccessKeySecret", HANG_ZHOUE_PUBLIC_ENDPOINT);
        Assert.ThrowsException<ArgumentOutOfRangeException>(() => options.Sts = new AliyunStsOptions()
        {
            EarlyExpires = -1
        });
    }
}
