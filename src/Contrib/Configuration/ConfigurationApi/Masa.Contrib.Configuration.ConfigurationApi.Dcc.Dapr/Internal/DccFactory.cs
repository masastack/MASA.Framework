// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Dapr.Internal;

internal static class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        JsonSerializerOptions jsonSerializerOptions,
        DccDaprOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new DaprConfigurationApiClient(serviceProvider,
            jsonSerializerOptions,
            defaultSectionOption,
            expandSectionOptions);
    }

    public static IConfigurationApiManage CreateManage(
        ICaller caller,
        DccDaprOptions defaultSectionOption,
        JsonSerializerOptions jsonSerializerOptions,
        List<DccSectionOptions>? expandSectionOptions)
        => new DaprConfigurationApiManage(caller, defaultSectionOption, jsonSerializerOptions, expandSectionOptions);
}
