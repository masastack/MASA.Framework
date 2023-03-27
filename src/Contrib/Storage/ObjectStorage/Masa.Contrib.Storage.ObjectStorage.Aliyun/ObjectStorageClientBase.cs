// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public abstract class ObjectStorageClientBase : AbstractStorageClient
{
    protected readonly ICredentialProvider CredentialProvider;
    protected readonly AliyunStorageOptions AliyunStorageOptions;

    protected ObjectStorageClientBase(
        AliyunStorageOptions aliyunStorageOptions,
        IMemoryCache memoryCache,
        ICredentialProvider? credentialProvider,
        IOssClientFactory? ossClientFactory = null,
        ILoggerFactory? loggerFactory = null
    )
    {
        AliyunStorageOptions = aliyunStorageOptions;
        CredentialProvider = credentialProvider ?? new DefaultCredentialProvider(aliyunStorageOptions, memoryCache, ossClientFactory, loggerFactory);
    }

    [ExcludeFromCodeCoverage]
    protected virtual IOss GetClient()
    {
        var credential = GetCredential();
        return new OssClient(AliyunStorageOptions.Endpoint, credential.AccessKeyId, credential.AccessKeySecret, credential.SecurityToken);
    }

    private (string AccessKeyId, string AccessKeySecret, string? SecurityToken) GetCredential()
    {
        if (AliyunStorageOptions.IsIncompleteStsOptions())
            return new(AliyunStorageOptions.AccessKeyId, AliyunStorageOptions.AccessKeySecret, null);

        var securityToken = CredentialProvider.GetSecurityToken();
        return new(securityToken.AccessKeyId, securityToken.AccessKeySecret, securityToken.SessionToken);
    }
}
