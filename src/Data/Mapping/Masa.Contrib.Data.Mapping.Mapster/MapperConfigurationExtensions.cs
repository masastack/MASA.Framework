// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.Mapping.Mapster;

public static class MapperConfigurationExtensions
{
    public static MapperConfiguration AddMapping(this MapperConfiguration mapperConfiguration)
        => mapperConfiguration.AddMapping(MapMode.Shared);

    public static MapperConfiguration AddMapping(this MapperConfiguration mapperConfiguration, MapMode mode)
        => mapperConfiguration.AddMapping(new MapOptions()
        {
            Mode = mode
        });

    public static MapperConfiguration AddMapping(this MapperConfiguration mapperConfiguration, MapOptions mapOptions)
    {
        mapperConfiguration.ConfigurationMapperOptions(new MapsterOptionsExtension(mapOptions));
        return mapperConfiguration;
    }
}
