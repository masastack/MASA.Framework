// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class StorageOptions
{
    private static readonly List<KeyValuePair<string, string>> _bucketNames = new();

    public BucketNames BucketNames { get; set; }

    public StorageOptions() => BucketNames = new BucketNames(_bucketNames);

    public void TryAddBucketName(string name, string bucketName)
    {
        if (_bucketNames.All(item => item.Key != name))
            _bucketNames.Add(new KeyValuePair<string, string>(name, bucketName));
    }
}
