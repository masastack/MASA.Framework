// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster.Internal;

internal static class TypeAdapterSetterExpand
{
    public static TypeAdapterSetter<TSource, TDestination> NewConfigByConstructor<TSource, TDestination>(TypeAdapterConfig adapterConfig,
        object constructorInfo)
    {
        return adapterConfig
            .NewConfig<TSource, TDestination>()
            .MapToConstructor((constructorInfo as ConstructorInfo)!);
    }
}
