// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public interface IOssClientFactory
{
    IOss GetClient(string accessKeyId, string accessKeySecret, string? securityToken, string endpoint);

    IAcsClient GetAcsClient(string accessKeyId, string accessKeySecret, string regionId);
}
