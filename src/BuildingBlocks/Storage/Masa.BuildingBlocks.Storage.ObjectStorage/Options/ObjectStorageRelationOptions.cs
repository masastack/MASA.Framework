// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class ObjectStorageRelationOptions : MasaRelationOptions<IObjectStorageClient>
{
    public ObjectStorageRelationOptions(string name) : base(name)
    {
    }

    public ObjectStorageRelationOptions(string name, Func<IServiceProvider, IObjectStorageClient> func) : this(name)
    {
        Func = func;
    }
}
