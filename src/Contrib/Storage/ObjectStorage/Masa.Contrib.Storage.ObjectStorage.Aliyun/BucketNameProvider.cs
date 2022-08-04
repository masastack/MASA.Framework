// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Storage.ObjectStorage.Aliyun;

public class BucketNameProvider : IBucketNameProvider
{
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public BucketNameProvider(IOptionsMonitor<StorageOptions> storageOptions)
        => _storageOptions = storageOptions;

    public string GetBucketName()
        => _storageOptions.CurrentValue.BucketNames.DefaultBucketName;

    public string GetBucketName<TContainer>() where TContainer : class
        => _storageOptions.CurrentValue.BucketNames.GetBucketName(BucketNameAttribute.GetName<TContainer>());
}
