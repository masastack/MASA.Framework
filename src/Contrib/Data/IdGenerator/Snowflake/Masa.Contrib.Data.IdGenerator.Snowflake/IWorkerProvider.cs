// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.Data.IdGenerator.Snowflake;

public interface IWorkerProvider
{
    /// <summary>
    /// Working machine id
    /// </summary>
    Task<long> GetWorkerIdAsync();

    /// <summary>
    /// Refresh workid activity status
    /// </summary>
    /// <returns></returns>
    Task RefreshAsync();

    /// <summary>
    /// logout workid active status
    /// </summary>
    /// <returns></returns>
    Task LogOutAsync();
}
