// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomizeCredentialProvider : DefaultCredentialProvider
{
    public readonly TemporaryCredentialsResponse TemporaryCredentials = new(
        "accessKeyId",
        "secretAccessKey",
        "sessionToken",
        DateTime.UtcNow.AddHours(-1));

    public CustomizeCredentialProvider(IOssClientFactory ossClientFactory, AliyunStorageOptions options, IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger) : base(ossClientFactory, options, cache, logger)
    {
    }

    public CustomizeCredentialProvider(IOssClientFactory ossClientFactory, IOptionsMonitor<AliyunStorageOptions> options, IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger) : base(ossClientFactory, options, cache, logger)
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

    public void TestExpirationTimeLessThan10Second(int durationSeconds)
    {
        var expirationTime = DateTime.UtcNow.AddSeconds(-durationSeconds);
        base.SetTemporaryCredentials(new TemporaryCredentialsResponse("accessKeyId", "secretAccessKey", "sessionToken", expirationTime));
    }
}
