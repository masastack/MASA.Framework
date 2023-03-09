// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class DefaultObjectStorageClientContainerFactory : IObjectStorageClientContainerFactory
{
    private readonly IObjectStorageClientFactory _objectStorageClientFactory;
    private readonly IBucketNameFactory _bucketNameFactory;

    public DefaultObjectStorageClientContainerFactory(
        IObjectStorageClientFactory objectStorageClientFactory,
        IBucketNameFactory bucketNameFactory)
    {
        _objectStorageClientFactory = objectStorageClientFactory;
        _bucketNameFactory = bucketNameFactory;
    }

    public IObjectStorageClientContainer Create()
        => new DefaultObjectStorageClientContainer(_objectStorageClientFactory.Create(), _bucketNameFactory.Create().GetBucketName());

    public IObjectStorageClientContainer Create(string name)
        => new DefaultObjectStorageClientContainer(_objectStorageClientFactory.Create(name),
            _bucketNameFactory.Create(name).GetBucketName());

    public IObjectStorageClientContainer<TContainer> Create<TContainer>() where TContainer : class
        => new DefaultObjectStorageClientContainer<TContainer>(_objectStorageClientFactory.Create(), _bucketNameFactory.Create());

    public IObjectStorageClientContainer<TContainer> Create<TContainer>(string name) where TContainer : class
        => new DefaultObjectStorageClientContainer<TContainer>(_objectStorageClientFactory.Create(name), _bucketNameFactory.Create(name));
}
