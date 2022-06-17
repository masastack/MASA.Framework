// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public interface IAliyunStorageOptionProvider
{
    bool SupportCallback { get; }
    
    bool IncompleteStsOptions { get; }

    AliyunStorageOptions GetOptions();
}
