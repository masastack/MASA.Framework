// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Data;

public interface IConnectionStringProvider
{
    /// <summary>
    /// Get Database Connection Strings based on ConnectionName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    Task<string> GetConnectionStringAsync(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME);

    /// <summary>
    /// Get Database Connection Strings based on ConnectionName
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    string GetConnectionString(string name = ConnectionStrings.DEFAULT_CONNECTION_STRING_NAME);
}
