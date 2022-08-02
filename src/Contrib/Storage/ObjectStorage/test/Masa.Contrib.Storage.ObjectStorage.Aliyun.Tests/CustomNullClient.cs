// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

public class CustomNullClient : DefaultCredentialProvider
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


    public CustomNullClient(IOssClientFactory ossClientFactory,
        IAliyunStorageOptionProvider optionProvider,
        IMemoryCache cache,
        ILogger<DefaultCredentialProvider>? logger = null)
        : base(ossClientFactory, optionProvider, cache, logger)
    {
    }
}
