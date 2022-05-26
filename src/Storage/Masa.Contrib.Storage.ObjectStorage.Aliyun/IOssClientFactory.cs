// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public interface IOssClientFactory
{
    IOss GetClient(string AccessKeyId, string AccessKeySecret, string? SecurityToken, string endpoint);

    IAcsClient GetAcsClient(string AccessKeyId, string AccessKeySecret, string regionId);
}
