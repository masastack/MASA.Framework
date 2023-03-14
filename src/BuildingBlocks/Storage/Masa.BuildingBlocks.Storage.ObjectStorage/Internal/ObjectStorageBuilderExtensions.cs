// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

[assembly: InternalsVisibleTo("Masa.Contrib.Storage.ObjectStorage.Aliyun")]

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

internal static class ObjectStorageBuilderExtensions
{
    public static void AddObjectStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        string name,
        Func<IServiceProvider, IObjectStorageClient> implementationFactory,
        Func<IServiceProvider, IBucketNameProvider> bucketNameImplementationFactory)
    {
        objectStorageBuilder.Services.Configure<ObjectStorageFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The ObjectStorage name already exists, please change the name, the repeat name is [{name}]");

            callerOptions.Options.Add(new(name, implementationFactory));
        });

        objectStorageBuilder.Services.Configure<BucketNameFactoryOptions>(callerOptions =>
        {
            if (callerOptions.Options.Any(relation => relation.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The Bucket name already exists, please change the name, the repeat name is [{name}]");

            callerOptions.Options.Add(new(name, bucketNameImplementationFactory));
        });
    }
}
