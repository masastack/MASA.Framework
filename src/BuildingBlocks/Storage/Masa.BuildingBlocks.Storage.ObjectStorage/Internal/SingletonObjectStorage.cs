// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

internal class SingletonObjectStorage
{
    public IObjectStorageClient ObjectStorageClient { get; }

    public IBucketNameProvider BucketNameProvider { get; }

    public SingletonObjectStorage(IObjectStorageClient objectStorageClient, IBucketNameProvider bucketNameProvider)
    {
        ObjectStorageClient = objectStorageClient;
        BucketNameProvider = bucketNameProvider;
    }
}
