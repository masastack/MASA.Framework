// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun.Options;

public class AliyunStorageConfigureOptions : AliyunOptions
{
    public AliyunStorageOptions Storage { get; set; } = new();

    /// <summary>
    /// Aliyun STS configuration, it is not necessarily a storage configuration
    ///
    /// </summary>
    public AliyunStsOptions? Sts { get; set; }
}
