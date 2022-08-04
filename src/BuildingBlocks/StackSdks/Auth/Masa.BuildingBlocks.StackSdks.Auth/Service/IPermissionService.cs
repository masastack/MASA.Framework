// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.BuildingBlocks.StackSdks.Auth.Service;

public interface IPermissionService
{
    /// <summary>
    /// Get menus for the appid with current user
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    Task<List<MenuModel>> GetMenusAsync(string appId);

    /// <summary>
    /// Get element permissions for the appid with current user
    /// </summary>
    /// <param name="appId"></param>
    /// <returns></returns>
    Task<List<string>> GetElementPermissionsAsync(string appId);

    Task<bool> AuthorizedAsync(string appId, string code);

    Task<bool> AddFavoriteMenuAsync(Guid menuId);

    Task<bool> RemoveFavoriteMenuAsync(Guid menuId);

    Task<List<CollectMenuModel>> GetFavoriteMenuListAsync();
}
