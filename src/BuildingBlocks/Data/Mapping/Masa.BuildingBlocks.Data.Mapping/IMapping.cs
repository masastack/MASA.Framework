// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Mapping;

public interface IMapper
{
    TDestination Map<TSource, TDestination>(TSource source, MapOptions? options = null);

    TDestination Map<TDestination>(object source, MapOptions? options = null);

    TDestination Map<TSource, TDestination>(TSource source, TDestination destination, MapOptions? options = null);
}
