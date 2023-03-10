// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

namespace Masa.BuildingBlocks.Data;

public class MasaFactoryOptions<TRelationOptions> where TRelationOptions : MasaRelationOptions
{
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Singleton;

    public List<TRelationOptions> Options { get; set; } = new();
}
