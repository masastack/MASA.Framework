// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class ObjectStorageBuilder
{
    public string Name { get; }

    public IServiceCollection Services { get; }

    public ObjectStorageBuilder(IServiceCollection services, string name)
    {
        Services = services;
        Name = name;
    }
}
