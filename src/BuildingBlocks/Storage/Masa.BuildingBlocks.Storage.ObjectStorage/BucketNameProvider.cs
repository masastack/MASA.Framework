// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class BucketNameProvider :
    IBucketNameProviderSingleton,
    IBucketNameProviderScoped,
    IBucketNameProviderTransient
{
    public BucketNames BucketNames { get; set; }

    public BucketNameProvider(BucketNames bucketNames)
        => BucketNames = bucketNames;

    public string GetBucketName()
        => BucketNames.DefaultBucketName;

    public string GetBucketName<TContainer>() where TContainer : class
        => BucketNames.GetBucketName(BucketNameAttribute.GetName<TContainer>());
}
