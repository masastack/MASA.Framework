// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Internal;

internal class DccFactory
{
    public static IConfigurationApiClient CreateClient(
        IServiceProvider serviceProvider,
        IMemoryCacheClient client,
        JsonSerializerOptions jsonSerializerOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
    {
        return new ConfigurationApiClient(serviceProvider, client, jsonSerializerOptions, defaultSectionOption, expandSectionOptions);
    }

    public static IConfigurationApiManage CreateManage(
        ICallerProvider callerProvider,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        => new ConfigurationApiManage(callerProvider, defaultSectionOption, expandSectionOptions);
}
