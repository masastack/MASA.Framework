// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

internal class CustomCredentialProvider : DefaultCredentialProvider
{
    public readonly TemporaryCredentialsResponse TemporaryCredentials = new(
        "accessKeyId",
        "secretAccessKey",
        "sessionToken",
        DateTime.UtcNow.AddHours(-1));

    public CustomCredentialProvider(
        AliyunStorageOptions aliyunStorageOptions,
        IMemoryCache cache,
        IOssClientFactory? ossClientFactory = null,
        ILoggerFactory? loggerFactory = null)
        : base(aliyunStorageOptions, cache, ossClientFactory, loggerFactory)
    {
    }

    public override TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKeyId,
        string accessKeySecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds) => TemporaryCredentials;

    public TemporaryCredentialsResponse TestGetTemporaryCredentials(string regionId,
        string accessKeyId,
        string accessKeySecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds)
        => base.GetTemporaryCredentials(regionId, accessKeyId, accessKeySecret, roleArn, roleSessionName, policy, durationSeconds);

    public void TestExpirationTimeLessThan(int durationSeconds)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(-durationSeconds);
        base.SetTemporaryCredentials(new TemporaryCredentialsResponse("accessKeyId", "secretAccessKey", "sessionToken", expirationTime));
    }
}
