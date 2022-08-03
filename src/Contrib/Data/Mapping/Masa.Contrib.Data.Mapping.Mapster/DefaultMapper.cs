// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public class DefaultMapper : IMapper
{
    private readonly IMappingConfigProvider _provider;

    public DefaultMapper(IMappingConfigProvider provider)
        => _provider = provider;

    public TDestination Map<TSource, TDestination>(TSource source, MapOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        return source.Adapt<TSource, TDestination>(_provider.GetConfig(source.GetType(), typeof(TDestination), options));
    }

    public TDestination Map<TDestination>(object source, MapOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        return source.Adapt<TDestination>(_provider.GetConfig(source.GetType(), typeof(TDestination), options));
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, MapOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(source, nameof(source));

        Type destinationType = destination?.GetType() ?? typeof(TDestination);
        return source.Adapt<TSource, TDestination>(destination, _provider.GetConfig(source.GetType(), destinationType, options));
    }
}
