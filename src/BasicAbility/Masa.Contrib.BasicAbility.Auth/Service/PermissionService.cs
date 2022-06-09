// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class PermissionService : IPermissionService
{
    readonly ICallerProvider _callerProvider;

    const string PARTY = "api/permission/";

    public PermissionService(ICallerProvider callerProvider)
    {
        _callerProvider = callerProvider;
    }

    //todo remove userId param
    public async Task<bool> AuthorizedAsync(string appId, string code, Guid userId)
    {
        var requestUri = $"{PARTY}authorized?appId={appId}&code={code}&userId={userId}";
        return await _callerProvider.GetAsync<bool>(requestUri);
    }

    public async Task<List<MenuModel>> GetMenusAsync(string appId, Guid userId)
    {
        var requestUri = $"{PARTY}menus?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<MenuModel>>(requestUri, default) ?? new();
    }

    public async Task<List<string>> GetElementPermissionsAsync(string appId, Guid userId)
    {
        var requestUri = $"{PARTY}element-permissions?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<string>>(requestUri, default) ?? new();
    }
}
