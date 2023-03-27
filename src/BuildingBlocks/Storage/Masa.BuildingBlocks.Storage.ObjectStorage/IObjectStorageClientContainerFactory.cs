// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public interface IObjectStorageClientContainerFactory : IMasaFactory<IObjectStorageClientContainer>
{
    IObjectStorageClientContainer<TContainer> Create<TContainer>() where TContainer : class;

    IObjectStorageClientContainer<TContainer> Create<TContainer>(string name) where TContainer : class;
}
