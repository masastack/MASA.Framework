// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class PermissionService : IPermissionService
{
    readonly ICallerProvider _callerProvider;
    readonly IUserContext _userContext;

    const string PART = "api/permission/";

    public PermissionService(ICallerProvider callerProvider, IUserContext userContext)
    {
        _callerProvider = callerProvider;
        _userContext = userContext;
    }

    //todo remove userId param
    public async Task<bool> AuthorizedAsync(string appId, string code)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PART}authorized?appId={appId}&code={code}&userId={userId}";
        return await _callerProvider.GetAsync<bool>(requestUri);
    }

    public async Task<List<MenuModel>> GetMenusAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PART}menus?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<MenuModel>>(requestUri, default) ?? new();
    }

    public async Task<List<string>> GetElementPermissionsAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PART}element-permissions?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<string>>(requestUri, default) ?? new();
    }

    public async Task<bool> AddFavoriteMenuAsync(Guid menuId)
    {
        try
        {
            var userId = _userContext.GetUserId<Guid>();
            await _callerProvider.PutAsync($"{PART}addFavoriteMenu?permissionId={menuId}&userId={userId}", null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RemoveFavoriteMenuAsync(Guid menuId)
    {
        try
        {
            var userId = _userContext.GetUserId<Guid>();
            await _callerProvider.PutAsync($"{PART}removeFavoriteMenu?permissionId={menuId}&userId={userId}", null);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<CollectMenuModel>> GetFavoriteMenuListAsync()
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PART}menu-favorite-list?userId={userId}";
        return await _callerProvider.GetAsync<List<CollectMenuModel>>(requestUri, default) ?? new();
    }
}
