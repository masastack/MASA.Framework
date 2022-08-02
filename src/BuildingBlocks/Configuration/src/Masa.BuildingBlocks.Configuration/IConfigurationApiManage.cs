// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Configuration;

public interface IConfigurationApiManage
{
    /// <summary>
    /// Initialize config object
    /// </summary>
    /// <param name="environment">Environment name</param>
    /// <param name="cluster">Cluster name</param>
    /// <param name="appId">App id</param>
    /// <param name="configObjects">Config objects,Key:config object name,Value:config object content</param>
    /// <returns></returns>
    Task InitializeAsync(string environment, string cluster, string appId, Dictionary<string, string> configObjects);

    Task UpdateAsync(string environment, string cluster, string appId, string configObject, object value);
}
