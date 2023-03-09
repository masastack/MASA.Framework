// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultObjectStorageClientFactory : MasaFactoryBase<IObjectStorageClient, ObjectStorageRelationOptions>, IObjectStorageClientFactory
{
    // private readonly IObjectStorageClient _objectStorageClient;
    // private readonly IBucketNameProvider _bucketNameProvider;
    // private readonly IOptionsMonitor<StorageOptions> _storageOptions;
    //
    // public DefaultObjectStorageClientFactory(
    //     IObjectStorageClient objectStorageClient,
    //     IBucketNameProvider bucketNameProvider,
    //     IOptionsMonitor<StorageOptions> storageOptions)
    // {
    //     _objectStorageClient = objectStorageClient;
    //     _bucketNameProvider = bucketNameProvider;
    //     _storageOptions = storageOptions;
    // }

    protected override string DefaultServiceNotFoundMessage => "No default ObjectStorage found";
    protected override string SpecifyServiceNotFoundMessage => "Please make sure you have used [{0}] ObjectStorage, it was not found";

    protected override MasaFactoryOptions<ObjectStorageRelationOptions> FactoryOptions => _options.CurrentValue;

    private readonly IOptionsMonitor<ObjectStorageFactoryOptions> _options;

    public DefaultObjectStorageClientFactory(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _options = serviceProvider.GetRequiredService<IOptionsMonitor<ObjectStorageFactoryOptions>>();
    }

    // public IObjectStorageClientContainer Create()
    //     => new DefaultObjectStorageClientContainer(
    //         _objectStorageClient,
    //         _bucketNameProvider.GetBucketName());
    //
    // public IObjectStorageClientContainer Create(string name)
    //     => new DefaultObjectStorageClientContainer(
    //         _objectStorageClient,
    //         _storageOptions.CurrentValue.BucketNames.GetBucketName(name));
}
