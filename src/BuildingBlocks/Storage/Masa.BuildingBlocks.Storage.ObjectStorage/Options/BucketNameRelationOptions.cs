// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Storage.ObjectStorage;

public class BucketNameRelationOptions : MasaRelationOptions<IBucketNameProvider>
{
    public BucketNameRelationOptions(string name) : base(name)
    {
    }

    public BucketNameRelationOptions(string name, Func<IServiceProvider, IBucketNameProvider> func) : this(name)
    {
        Func = func;
    }
}
