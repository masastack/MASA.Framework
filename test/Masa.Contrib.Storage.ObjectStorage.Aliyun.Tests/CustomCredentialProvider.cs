// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomCredentialProvider : DefaultCredentialProvider
{
    public readonly TemporaryCredentialsResponse TemporaryCredentials = new(
        "accessKeyId",
        "secretAccessKey",
        "sessionToken",
        DateTime.UtcNow.AddHours(-1));

    public CustomCredentialProvider(IOssClientFactory ossClientFactory,
        IAliyunStorageOptionProvider optionProvider,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger)
        : base(ossClientFactory, optionProvider, cache, logger)
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
