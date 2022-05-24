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
    /// By default, the status of the workid is refreshed every 30 seconds to ensure that the workid will not be removed.
    /// </summary>
    public const int DEFAULT_HEARTBEATINTERVAL = 30 * 1000;
}
