// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Redis.Internal;

internal static class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccRedisOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new RedisConfigurationApiClient(serviceProvider,
            jsonSerializerOptions,
            dccOptions,
            defaultSectionOption,
            expandSectionOptions);
    }

    public static IConfigurationApiManage CreateManage(
        ICaller caller,
        DccSectionOptions defaultSectionOption,
        JsonSerializerOptions jsonSerializerOptions,
        List<DccSectionOptions>? expandSectionOptions)
        => new RedisConfigurationApiManage(caller, defaultSectionOption, jsonSerializerOptions, expandSectionOptions);
}
