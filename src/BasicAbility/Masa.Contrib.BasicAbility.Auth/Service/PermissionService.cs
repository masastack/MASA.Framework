// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Contrib.BasicAbility.Auth.Service;

public class PermissionService : IPermissionService
{
    readonly ICallerProvider _callerProvider;
    readonly IUserContext _userContext;

    const string PARTY = "api/permission/";

    public PermissionService(ICallerProvider callerProvider, IUserContext userContext)
    {
        _callerProvider = callerProvider;
        _userContext = userContext;
    }

    //todo remove userId param
    public async Task<bool> AuthorizedAsync(string appId, string code)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PARTY}authorized?appId={appId}&code={code}&userId={userId}";
        return await _callerProvider.GetAsync<bool>(requestUri);
    }

    public async Task<List<MenuModel>> GetMenusAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        //var userId = Guid.Parse("D7A85879-8229-4297-861E-08DA4EB68F74");
        var requestUri = $"{PARTY}menus?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<MenuModel>>(requestUri, default) ?? new();
    }

    public async Task<List<string>> GetElementPermissionsAsync(string appId)
    {
        var userId = _userContext.GetUserId<Guid>();
        var requestUri = $"{PARTY}element-permissions?appId={appId}&userId={userId}";
        return await _callerProvider.GetAsync<List<string>>(requestUri, default) ?? new();
    }
}
