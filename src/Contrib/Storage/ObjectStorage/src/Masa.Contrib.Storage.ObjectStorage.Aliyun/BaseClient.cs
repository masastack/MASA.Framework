// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public abstract class BaseClient
{
    protected readonly ICredentialProvider CredentialProvider;
    protected IAliyunStorageOptionProvider OptionProvider { get; }
    protected AliyunStorageOptions Options => OptionProvider.GetOptions();

    public BaseClient(ICredentialProvider credentialProvider,
        IAliyunStorageOptionProvider optionProvider)
    {
        CredentialProvider = credentialProvider;
        OptionProvider = optionProvider;
    }

    public virtual IOss GetClient()
    {
        var credential = GetCredential();
        return new OssClient(Options.Endpoint, credential.AccessKeyId, credential.AccessKeySecret, credential.SecurityToken);
    }

    public virtual (string AccessKeyId, string AccessKeySecret, string? SecurityToken) GetCredential()
    {
        if (OptionProvider.IncompleteStsOptions)
            return new(Options.AccessKeyId, Options.AccessKeySecret, null);

        var securityToken = CredentialProvider.GetSecurityToken();
        return new(securityToken.AccessKeyId, securityToken.AccessKeySecret, securityToken.SessionToken);
    }
}
