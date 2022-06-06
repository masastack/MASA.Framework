// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomizeNullClient : DefaultCredentialProvider
{
    public string Message = "You are not authorized to do this action. You should be authorized by RAM.";

    public override TemporaryCredentialsResponse GetTemporaryCredentials(
        string regionId,
        string accessKeyId,
        string accessKeySecret,
        string roleArn,
        string roleSessionName,
        string policy,
        long durationSeconds) => throw new Exception(Message);

    public CustomizeNullClient(IOssClientFactory ossClientFactory, AliyunStorageOptions options, IMemoryCache cache, ILogger<DefaultCredentialProvider>? logger) : base(ossClientFactory, options, cache, logger)
    {
    }

    public CustomizeNullClient(IOssClientFactory ossClientFactory, IOptionsMonitor<AliyunStorageOptions> options, IMemoryCache cache, ILogger<DefaultCredentialProvider>? logger) : base(ossClientFactory, options, cache, logger)
    {
    }
}
