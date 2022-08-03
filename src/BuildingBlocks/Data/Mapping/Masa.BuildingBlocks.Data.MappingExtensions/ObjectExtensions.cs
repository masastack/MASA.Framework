// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace System;

public static class ObjectExtensions
{
    private static Mapper? _mapper;
    private static Mapper GetMapper() => _mapper ??= Mapper.Instance ?? throw new Exception("Please use MapperFactory to initialize Mapper");

    public static TDestination Map<TSource, TDestination>(this TSource source, MapOptions? options = null)
        => GetMapper().Map<TSource, TDestination>(source, options);

    public static TDestination Map<TDestination>(this object obj, MapOptions? options = null)
        => GetMapper().Map<TDestination>(obj, options);

    public static TDestination Map<TSource, TDestination>(this TSource source, TDestination destination, MapOptions? options = null)
        => GetMapper().Map(source, destination, options);
}
