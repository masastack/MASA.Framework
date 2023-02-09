// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultObjectStorageClientFactory : IObjectStorageClientFactory
{
    private readonly IObjectStorageClient _objectStorageClient;
    private readonly IBucketNameProvider _bucketNameProvider;
    private readonly IOptionsMonitor<StorageOptions> _storageOptions;

    public DefaultObjectStorageClientFactory(
        IObjectStorageClient objectStorageClient,
        IBucketNameProvider bucketNameProvider,
        IOptionsMonitor<StorageOptions> storageOptions)
    {
        _objectStorageClient = objectStorageClient;
        _bucketNameProvider = bucketNameProvider;
        _storageOptions = storageOptions;
    }

    public IObjectStorageClientContainer Create()
        => new DefaultObjectStorageClientContainer(
            _objectStorageClient,
            _bucketNameProvider.GetBucketName());

    public IObjectStorageClientContainer Create(string name)
        => new DefaultObjectStorageClientContainer(
            _objectStorageClient,
            _storageOptions.CurrentValue.BucketNames.GetBucketName(name));
}
