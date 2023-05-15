// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Tests;

internal class CustomExceptionCredentialProvider : DefaultCredentialProvider
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


    public CustomExceptionCredentialProvider(
        AliyunStorageOptions aliyunStorageOptions,
        IMemoryCache cache,
        IOssClientFactory? ossClientFactory = null,
        ILoggerFactory? loggerFactory = null)
        : base(aliyunStorageOptions, cache, ossClientFactory, loggerFactory)
    {
    }
}
