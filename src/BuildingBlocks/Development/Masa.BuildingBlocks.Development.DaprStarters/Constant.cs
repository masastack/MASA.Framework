// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Development.DaprStarters;

public static class Constant
{
    public const string DEFAULT_APPID_DELIMITER = "-";

    public const string DEFAULT_FILE_NAME = "dapr";

    public const string DEFAULT_ARGUMENT_PREFIX = "--";

    /// <summary>
    /// Heartbeat detection interval, used to detect dapr status
    /// </summary>
    public const int DEFAULT_HEARTBEAT_INTERVAL = 5000;

    /// <summary>
    /// Default number of retries
    /// </summary>
    public const int DEFAULT_RETRY_TIME = 10;
}
