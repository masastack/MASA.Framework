// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public static class MapperExtensions
{
    public static MapperFactory AddMapping(this MapperFactory factory)
        => factory.AddMapping(MapMode.Shared);

    public static MapperFactory AddMapping(this MapperFactory factory, MapMode mode)
        => factory.AddMapping(new MapOptions()
        {
            Mode = mode
        });

    public static MapperFactory AddMapping(this MapperFactory factory, MapOptions mapOptions)
    {
        factory.ConfigurationMapperOptions(new MapsterExtension(mapOptions));
        return factory;
    }
}
