// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

[Serializable]
public class BucketNames : Dictionary<string, string>
{
    public const string DEFAULT_BUCKET_NAME = "DefaultBucketName";

    public string DefaultBucketName
    {
        get => GetBucketName(DEFAULT_BUCKET_NAME);
        set => this[DEFAULT_BUCKET_NAME] = value;
    }

    public BucketNames() { }
    public BucketNames(IEnumerable<KeyValuePair<string, string>> names) : base(names) { }

    public string GetBucketName(string name)
    {
        if (base.TryGetValue(name, out var bucketName))
            return bucketName;

        return name;
    }
}
