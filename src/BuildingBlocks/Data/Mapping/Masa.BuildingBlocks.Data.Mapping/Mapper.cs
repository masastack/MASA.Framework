// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data.Mapping;

public sealed class Mapper
{
    public static Mapper? Instance { get; set; }

    private readonly IMapper _mapper;

    public Mapper(IServiceCollection services) => _mapper = services.BuildServiceProvider().GetRequiredService<IMapper>();

    public TDestination Map<TSource, TDestination>(TSource source, MapOptions? options = null)
        => _mapper.Map<TSource, TDestination>(source, options);

    public TDestination Map<TDestination>(object source, MapOptions? options = null)
        => _mapper.Map<TDestination>(source, options);

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, MapOptions? options = null)
        => _mapper.Map(source, destination, options);
}
