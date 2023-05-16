// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Configuration.ConfigurationApi.Dcc.Tests.Internal;

internal class CustomConfigurationApiClient : ConfigurationApiClient
{
    public CustomConfigurationApiClient(
        IMultilevelCacheClient multilevelCacheClient,
        JsonSerializerOptions jsonSerializerOptions,
        DccConfigurationOptions dccConfigurationOptions)
        : base(multilevelCacheClient, jsonSerializerOptions, dccConfigurationOptions)
    {
    }

    public (string Raw, ConfigurationTypes ConfigurationType) FormatRawByTest(PublishReleaseModel? publishRelease, string paramName)
        => base.FormatRaw(publishRelease, paramName);

    public Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawByKeyAsyncByTest(string key, Action<string>? valueChanged)
        => base.GetRawByKeyAsync(key, valueChanged);

    public Task<dynamic> GetDynamicAsyncByTest(string key, Action<string, dynamic, JsonSerializerOptions>? valueChanged)
        => base.GetDynamicAsync(key, valueChanged);
}
