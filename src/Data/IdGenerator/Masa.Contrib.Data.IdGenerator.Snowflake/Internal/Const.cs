// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake.Internal;

internal class Const
{
    /// <summary>
    /// Default working cluster idkey
    /// </summary>
    public const string DEFAULT_WORKER_ID_KEY = "WORKER_ID";

    /// <summary>
    /// By default, the status of the workid is refreshed every 3 seconds to ensure that the workid will not be removed.
    /// default: 3s
    /// </summary>
    public const int DEFAULT_HEARTBEAT_INTERVAL = 3 * 1000;

    /// <summary>
    /// If the interval for failure to obtain WorkerId exceeds 10s
    /// it is considered that the current service cannot provide a valid WorkerId
    /// and the use is temporarily stopped.
    /// default: 10s
    /// </summary>
    public const int DEFAULT_EXPIRATION_TIME = 10 * 1000;
}
