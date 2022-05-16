// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public abstract class BaseClient
{
    protected readonly ICredentialProvider _credentialProvider;
    protected readonly AliyunStorageOptions _options;

    public BaseClient(ICredentialProvider credentialProvider,
        AliyunStorageOptions options)
    {
        _credentialProvider = credentialProvider;
        _options = options;
    }

    public virtual IOss GetClient()
    {
        var credential = GetCredential();
        return new OssClient(_options.Endpoint, credential.AccessKeyId, credential.AccessKeySecret, credential.SecurityToken);
    }

    public virtual (string AccessKeyId, string AccessKeySecret, string? SecurityToken) GetCredential()
    {
        if (!_credentialProvider.SupportSts)
            return new(_options.AccessKeyId, _options.AccessKeySecret, null);

        var securityToken = _credentialProvider.GetSecurityToken();
        return new(securityToken.AccessKeyId, securityToken.AccessKeySecret, securityToken.SessionToken);
    }
}
