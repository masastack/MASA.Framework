// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IConfigurationApiClient
{
    Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string configObject, Action<string>? valueChanged = null);

    Task<(string Raw, ConfigurationTypes ConfigurationType)> GetRawAsync(string environment, string cluster, string appId, string configObject, Action<string>? valueChanged = null);

    Task<T> GetAsync<T>(string configObject, Action<T>? valueChanged = null);

    Task<T> GetAsync<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged = null);

    Task<dynamic> GetDynamicAsync(string environment, string cluster, string appId, string configObject, Action<dynamic>? valueChanged = null);

    Task<dynamic> GetDynamicAsync(string configObject);

    (string Raw, ConfigurationTypes ConfigurationType) GetRaw(string configObject, Action<string>? valueChanged = null);

    (string Raw, ConfigurationTypes ConfigurationType) GetRaw(string environment, string cluster, string appId, string configObject, Action<string>? valueChanged = null);

    T Get<T>(string configObject, Action<T>? valueChanged = null);

    T Get<T>(string environment, string cluster, string appId, string configObject, Action<T>? valueChanged = null);

    dynamic GetDynamic(string environment, string cluster, string appId, string configObject, Action<dynamic>? valueChanged = null);

    dynamic GetDynamic(string configObject);
}

