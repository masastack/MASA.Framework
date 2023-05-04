// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Development.DaprStarter;

public interface IDaprProvider
{
    /// <summary>
    /// Complete dapr appid
    /// </summary>
    /// <returns></returns>
    string CompletionAppId(string? appId = null,
        bool disableAppIdSuffix = false,
        string? appIdSuffix = null,
        string appIdDelimiter = DaprStarterConstant.DEFAULT_APPID_DELIMITER);
}
