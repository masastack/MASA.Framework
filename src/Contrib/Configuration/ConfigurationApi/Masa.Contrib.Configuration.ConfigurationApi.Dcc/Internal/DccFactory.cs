// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal static class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        IMultilevelCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new ConfigurationApiClient(serviceProvider, client, jsonSerializerOptions, dccOptions, defaultSectionOption, expandSectionOptions);
    }

    public static IConfigurationApiManage CreateManage(
        ICaller caller,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        => new ConfigurationApiManage(caller, defaultSectionOption, expandSectionOptions);
}
