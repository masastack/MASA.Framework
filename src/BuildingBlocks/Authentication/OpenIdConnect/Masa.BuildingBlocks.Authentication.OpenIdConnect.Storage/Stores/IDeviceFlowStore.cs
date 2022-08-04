// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.Authentication.OpenIdConnect.Storage.Stores;

/// <summary>
/// Interface for the device flow store
/// </summary>
public interface IDeviceFlowStore
{
    /// <summary>
    /// Stores the device authorization request.
    /// </summary>
    /// <param name="deviceCode">The device code.</param>
    /// <param name="userCode">The user code.</param>
    /// <param name="data">The data.</param>
    /// <returns></returns>
    Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCodeModel data);

    /// <summary>
    /// Finds device authorization by user code.
    /// </summary>
    /// <param name="userCode">The user code.</param>
    /// <returns></returns>
    Task<DeviceCodeModel> FindByUserCodeAsync(string userCode);

    /// <summary>
    /// Finds device authorization by device code.
    /// </summary>
    /// <param name="deviceCode">The device code.</param>
    Task<DeviceCodeModel> FindByDeviceCodeAsync(string deviceCode);

    /// <summary>
    /// Updates device authorization, searching by user code.
    /// </summary>
    /// <param name="userCode">The user code.</param>
    /// <param name="data">The data.</param>
    Task UpdateByUserCodeAsync(string userCode, DeviceCodeModel data);

    /// <summary>
    /// Removes the device authorization, searching by device code.
    /// </summary>
    /// <param name="deviceCode">The device code.</param>
    Task RemoveByDeviceCodeAsync(string deviceCode);
}
