// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests.Internal;

internal class CustomConfigurationApiClient : ConfigurationApiClient
{
    public CustomConfigurationApiClient(
        IServiceProvider serviceProvider,
        IMemoryCacheClient client,
        Masa.BuildingBlocks.Data.ISerializer yamlSerializer,
        Masa.BuildingBlocks.Data.IDeserializer yamlDeserializer,
        JsonSerializerOptions jsonSerializerOptions,
        DccOptions dccOptions,
        DccSectionOptions defaultSectionOption,
        List<DccSectionOptions>? expandSectionOptions)
        : base(serviceProvider, client, yamlSerializer, yamlDeserializer, jsonSerializerOptions, dccOptions, defaultSectionOption, expandSectionOptions)
    {
    }

    public (string Raw, ConfigurationTypes ConfigurationType) TestFormatRaw(string? raw, string paramName)
        => base.FormatRaw(raw, paramName);

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> TestGetRawByKeyAsync(string key, Action<string>? valueChanged)
        => base.GetRawByKeyAsync(key, valueChanged);

    public Task<dynamic> TestGetDynamicAsync(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged) => base.GetDynamicAsync(key, valueChanged);
}
