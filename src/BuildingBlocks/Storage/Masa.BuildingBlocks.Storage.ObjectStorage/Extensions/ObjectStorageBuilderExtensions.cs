// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public static class ObjectStorageBuilderExtensions
{
    public static void UseCustomObjectStorage(
        this ObjectStorageBuilder objectStorageBuilder,
        Func<IServiceProvider, IManualObjectStorageClient> implementationFactory,
        Func<IServiceProvider, IBucketNameProvider> bucketNameImplementationFactory)
    {
        objectStorageBuilder.Services.Configure<ObjectStorageFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name.Equals(objectStorageBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The ObjectStorage name already exists, please change the name, the repeat name is [{objectStorageBuilder.Name}]");

            factoryOptions.Options.Add(new(objectStorageBuilder.Name, implementationFactory));
        });

        objectStorageBuilder.Services.Configure<BucketNameFactoryOptions>(factoryOptions =>
        {
            if (factoryOptions.Options.Any(relation => relation.Name.Equals(objectStorageBuilder.Name, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException(
                    $"The Bucket name already exists, please change the name, the repeat name is [{objectStorageBuilder.Name}]");

            factoryOptions.Options.Add(new(objectStorageBuilder.Name, bucketNameImplementationFactory));
        });
    }
}
