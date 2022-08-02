// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class DefaultOssClientFactory : IOssClientFactory
{
    public IOss GetClient(string accessKeyId, string accessKeySecret, string? securityToken, string endpoint)
        => new OssClient(endpoint, accessKeyId, accessKeySecret, securityToken);

    public IAcsClient GetAcsClient(string accessKeyId, string accessKeySecret, string regionId)
    {
        IClientProfile profile = DefaultProfile.GetProfile(regionId, accessKeyId, accessKeySecret);
        return new DefaultAcsClient(profile);
    }
}
