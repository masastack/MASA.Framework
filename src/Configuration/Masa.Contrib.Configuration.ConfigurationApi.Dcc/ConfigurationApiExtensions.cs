// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public static class ConfigurationApiExtensions
{

    public static IConfiguration GetDefault(this IConfigurationApi configurationApi)
    {
        return configurationApi.Get(StaticConfig.AppId);
    }

    public static IConfiguration GetPublic(this IConfigurationApi configurationApi)
    {
        return configurationApi.Get(StaticConfig.PublicId);
    }
}
