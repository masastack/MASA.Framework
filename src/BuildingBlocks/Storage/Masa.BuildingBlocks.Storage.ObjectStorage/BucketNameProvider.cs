// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class BucketNameProvider : IBucketNameProvider
{
    private BucketNames _bucketNames;

    public BucketNameProvider(BucketNames bucketNames)
        => _bucketNames = bucketNames;

    public string GetBucketName()
        => _bucketNames.DefaultBucketName;

    public string GetBucketName(string aliasName)
        => _bucketNames.GetBucketName(aliasName);

    public string GetBucketName<TContainer>() where TContainer : class
        => _bucketNames.GetBucketName(BucketNameAttribute.GetName<TContainer>());
}
